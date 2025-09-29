// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project

using System.Data.SQLite;

public static class SQLiteDatabase
{
    public static SQLiteConnection Connect(string path)
    {
        var conn = new SQLiteConnection($"Data Source={path};Version=3;");
        conn.Open();

        using var pragma = conn.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON;";
        pragma.ExecuteNonQuery();

        return conn;
    }
}