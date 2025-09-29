// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project

using System;
using System.Collections.Generic;
using System.Data.SQLite;

public static class SupplierDb
{
    public static void CreateTable(SQLiteConnection conn)
    {
        const string sql = @"CREATE TABLE IF NOT EXISTS Suppliers(
                               SupplierId   INTEGER PRIMARY KEY AUTOINCREMENT,
                               SupplierName TEXT NOT NULL,
                               Phone        TEXT,
                               Email        TEXT
                             );";
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public static int Count(SQLiteConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Suppliers;";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // idempotent: ensures these exist without duplicates
    public static void SeedDefaults(SQLiteConnection conn)
    {
        EnsureSupplier(conn, "Default Supplier", "", "");
        EnsureSupplier(conn, "Prime Med Supply", "757-555-1100", "sales@primemed.com");
        EnsureSupplier(conn, "OfficeHub", "757-555-2200", "hello@officehub.com");
    }

    private static void EnsureSupplier(SQLiteConnection conn, string name, string phone, string email)
    {
        using var check = conn.CreateCommand();
        check.CommandText = "SELECT SupplierId FROM Suppliers WHERE SupplierName = @n LIMIT 1;";
        check.Parameters.AddWithValue("@n", name);
        var existing = check.ExecuteScalar();

        if (existing == null || existing == DBNull.Value)
        {
            using var insert = conn.CreateCommand();
            insert.CommandText = "INSERT INTO Suppliers (SupplierName, Phone, Email) VALUES (@n,@p,@e);";
            insert.Parameters.AddWithValue("@n", name);
            insert.Parameters.AddWithValue("@p", phone ?? string.Empty);
            insert.Parameters.AddWithValue("@e", email ?? string.Empty);
            insert.ExecuteNonQuery();
            Console.WriteLine($"[Seed] Added supplier: {name}");
        }
    }

    public static void Add(SQLiteConnection conn, Supplier s)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Suppliers(SupplierName,Phone,Email) VALUES(@n,@p,@e);";
        cmd.Parameters.AddWithValue("@n", s.SupplierName ?? string.Empty);
        cmd.Parameters.AddWithValue("@p", s.Phone ?? string.Empty);
        cmd.Parameters.AddWithValue("@e", s.Email ?? string.Empty);
        cmd.ExecuteNonQuery();
        s.SupplierId = (int)conn.LastInsertRowId;
    }

    public static List<Supplier> GetAll(SQLiteConnection conn)
    {
        var list = new List<Supplier>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT SupplierId,SupplierName,Phone,Email FROM Suppliers ORDER BY SupplierName;";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Supplier(
                r.IsDBNull(1) ? "" : r.GetString(1),
                r.IsDBNull(2) ? "" : r.GetString(2),
                r.IsDBNull(3) ? "" : r.GetString(3))
            { SupplierId = r.GetInt32(0) });
        }
        return list;
    }
}