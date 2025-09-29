// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project
// Main

using System;
using System.Data.SQLite;

class Program
{
    static void Main()
    {
        using var conn = SQLiteDatabase.Connect("ims.db");

        // Ensure schema
        SupplierDb.CreateTable(conn);
        ItemDb.CreateTable(conn);

        // Idempotent demo data (no duplicates)
        SupplierDb.SeedDefaults(conn);
        if (ItemDb.Count(conn) == 0) ItemDb.SeedIfEmpty(conn);

        // Hand off to console UI
        new ConsoleApp(conn).Run();
    }
}