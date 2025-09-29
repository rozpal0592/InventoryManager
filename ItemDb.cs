// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;

public static class ItemDb
{
    public static void CreateTable(SQLiteConnection conn)
    {
        const string sql = @"
        CREATE TABLE IF NOT EXISTS Items(
          ItemId            INTEGER PRIMARY KEY AUTOINCREMENT,
          Name              TEXT NOT NULL,
          Category          TEXT NOT NULL,            -- Generic | Medical | Office
          Quantity          INTEGER NOT NULL,
          ReorderPoint      INTEGER NOT NULL,
          ExpirationDate    TEXT,                     -- ISO yyyy-MM-dd or NULL
          Unit              TEXT,
          RequiresColdChain INTEGER,                  -- 0/1 or NULL
          LotNumber         TEXT,
          Model             TEXT,
          SupplierId        INTEGER NOT NULL,
          CreatedAt         TEXT NOT NULL,
          UpdatedAt         TEXT NOT NULL,
          FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId)
        );";
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public static int Count(SQLiteConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Items;";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // Seed few items if table is empty (uses suppliers by name)
    public static void SeedIfEmpty(SQLiteConnection conn)
    {
        if (Count(conn) > 0) return;

        var suppliers = SupplierDb.GetAll(conn);
        if (suppliers.Count == 0) return;

        var medSup = suppliers.Find(s => s.SupplierName.Contains("Prime")) ?? suppliers[0];
        var offSup = suppliers.Find(s => s.SupplierName.Contains("Office")) ?? suppliers[0];

        var med = new MedicalSupply("Sterile Gloves (M)", 45, 50, DateTime.Today.AddDays(25), medSup, "box", false, "LOT-GLV-0925");
        var off = new OfficeSupply("Printer Paper 8.5x11", 8, 10, null, offSup, "ream", "HP A4 Classic");
        var gen = new InventoryItem("Sanitizing Wipes", 12, 10, DateTime.Today.AddDays(120), medSup, "canister");

        Add(conn, med);
        Add(conn, off);
        Add(conn, gen);

        Console.WriteLine("[Seed] Items added.");
    }

    public static void Add(SQLiteConnection conn, InventoryBase it)
    {
        string category = it switch
        {
            MedicalSupply => "Medical",
            OfficeSupply  => "Office",
            _             => "Generic"
        };

        string? exp  = it.ExpirationDate?.ToString("yyyy-MM-dd");
        string? unit = (it as InventoryItem)?.Unit;
        int? cc      = (it as MedicalSupply)?.RequiresColdChain == true ? 1 : (int?)null;
        string? lot  = (it as MedicalSupply)?.LotNumber;
        string? model= (it as OfficeSupply )?.Model;

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
          INSERT INTO Items(Name,Category,Quantity,ReorderPoint,ExpirationDate,
                            Unit,RequiresColdChain,LotNumber,Model,
                            SupplierId,CreatedAt,UpdatedAt)
          VALUES(@n,@c,@q,@rop,@exp,@unit,@cc,@lot,@model,@sid,@ca,@ua);";
        cmd.Parameters.AddWithValue("@n",   it.Name);
        cmd.Parameters.AddWithValue("@c",   category);
        cmd.Parameters.AddWithValue("@q",   it.Quantity);
        cmd.Parameters.AddWithValue("@rop", it.ReorderPoint);
        cmd.Parameters.AddWithValue("@exp", (object?)exp ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@unit",(object?)unit ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@cc",  (object?)cc  ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@lot", (object?)lot ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@model",(object?)model ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@sid", it.Supplier.SupplierId);
        cmd.Parameters.AddWithValue("@ca",  it.CreatedAt.ToString("o", CultureInfo.InvariantCulture));
        cmd.Parameters.AddWithValue("@ua",  it.UpdatedAt.ToString("o", CultureInfo.InvariantCulture));
        cmd.ExecuteNonQuery();
    }

    public static List<InventoryBase> GetAll(SQLiteConnection conn, List<Supplier> suppliers)
    {
        var list = new List<InventoryBase>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT ItemId,Name,Category,Quantity,ReorderPoint,ExpirationDate,
                                   Unit,RequiresColdChain,LotNumber,Model,
                                   SupplierId,CreatedAt,UpdatedAt
                            FROM Items ORDER BY Name;";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int i = 0;
            int id        = r.GetInt32(i++);
            string name   = r.IsDBNull(i) ? "" : r.GetString(i); i++;
            string cat    = r.IsDBNull(i) ? "Generic" : r.GetString(i); i++;
            int qty       = r.GetInt32(i++);
            int rop       = r.GetInt32(i++);
            DateTime? exp = r.IsDBNull(i) ? (DateTime?)null : DateTime.Parse(r.GetString(i)); i++;
            string unit   = r.IsDBNull(i) ? "" : r.GetString(i); i++;
            bool cc       = !r.IsDBNull(i) && r.GetInt32(i) == 1; i++;
            string lot    = r.IsDBNull(i) ? "" : r.GetString(i); i++;
            string model  = r.IsDBNull(i) ? "" : r.GetString(i); i++;
            int sid       = r.GetInt32(i++);
            DateTime ca   = DateTime.Parse(r.GetString(i++), null, DateTimeStyles.RoundtripKind);
            DateTime ua   = DateTime.Parse(r.GetString(i++), null, DateTimeStyles.RoundtripKind);

            Supplier supplier = suppliers.Find(s => s.SupplierId == sid)
                                  ?? (suppliers.Count > 0 ? suppliers[0]
                                     : new Supplier("Unknown Supplier", "", "") { SupplierId = 0 });

            InventoryBase item = cat switch
            {
                "Medical" => new MedicalSupply(name, qty, rop, exp, supplier, unit, cc, lot),
                "Office"  => new OfficeSupply (name, qty, rop, exp, supplier, unit, model),
                _         => new InventoryItem(name, qty, rop, exp, supplier, unit)
            };

            typeof(InventoryBase).GetProperty(nameof(InventoryBase.ItemId))?.SetValue(item, id);
            item.GetType().GetProperty(nameof(InventoryBase.CreatedAt))?.SetValue(item, ca);
            item.GetType().GetProperty(nameof(InventoryBase.UpdatedAt))?.SetValue(item, ua);
            list.Add(item);
        }
        return list;
    }

    public static void UpdateQuantity(SQLiteConnection conn, int itemId, int newQty)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"UPDATE Items SET Quantity=@q, UpdatedAt=@ua WHERE ItemId=@id;";
        cmd.Parameters.AddWithValue("@q", newQty);
        cmd.Parameters.AddWithValue("@ua", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
        cmd.Parameters.AddWithValue("@id", itemId);
        cmd.ExecuteNonQuery();
    }

    public static void Delete(SQLiteConnection conn, int itemId)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"DELETE FROM Items WHERE ItemId=@id;";
        cmd.Parameters.AddWithValue("@id", itemId);
        cmd.ExecuteNonQuery();
    }
}