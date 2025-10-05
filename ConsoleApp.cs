// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project

using System;
using System.Collections.Generic;
using System.Data.SQLite;

public class ConsoleApp
{
    private readonly SQLiteConnection _conn;

    public ConsoleApp(SQLiteConnection conn) => _conn = conn;

    public void Run()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n==== Inventory Management ====");
            Console.WriteLine("1) List Suppliers");
            Console.WriteLine("2) Add Supplier");
            Console.WriteLine("3) List Items");
            Console.WriteLine("4) Add Generic Item");
            Console.WriteLine("5) Add Medical Supply");
            Console.WriteLine("6) Add Office Supply");
            Console.WriteLine("7) Update Item Quantity");
            Console.WriteLine("8) Delete Item");
            Console.WriteLine("0) Exit");
            string choice = Input("Select: ");

            switch (choice)
            {
                case "1": ListSuppliers(); break;
                case "2": AddSupplier(); break;
                case "3": ListItems(); break;
                case "4": AddGenericItem(); break;
                case "5": AddMedicalSupply(); break;
                case "6": AddOfficeSupply(); break;
                case "7": UpdateItemQuantity(); break;
                case "8": DeleteItem(); break;
                case "0": running = false; break;
                default:  Console.WriteLine("Invalid choice."); break;
            }
        }
    }

    // ---------- Suppliers ----------
    private void ListSuppliers()
    {
        var list = SupplierDb.GetAll(_conn);
        if (list.Count == 0) { Console.WriteLine("No suppliers."); return; }

        Console.WriteLine(new string('-', 90));
        Console.WriteLine($"{"ID",-4} {"Supplier",-30} {"Phone",-20} {"Email",-30}");
        Console.WriteLine(new string('-', 90));

        foreach (var s in list)
            Console.WriteLine($"{s.SupplierId,-4} {s.SupplierName,-30} {s.Phone,-20} {s.Email,-30}");

        Console.WriteLine(new string('-', 90));
    }

    private void AddSupplier()
    {
        string name  = Input("Supplier name: ", required: true);
        string phone = Input("Phone: ");
        string email = Input("Email: ");
        var s = new Supplier(name, phone, email);
        SupplierDb.Add(_conn, s);
        Console.WriteLine($"Added supplier #{s.SupplierId}: {s.SupplierName}");
    }

    private Supplier? ChooseSupplier()
    {
        var list = SupplierDb.GetAll(_conn);
        if (list.Count == 0) return null;

        Console.WriteLine("Suppliers:");
        foreach (var s in list) Console.WriteLine($"{s.SupplierId,3} | {s.SupplierName}");
        int id = InputInt("SupplierId: ");

        foreach (var s in list)
            if (s.SupplierId == id) return s;

        Console.WriteLine("Not found; using first supplier.");
        return list[0];
    }

    // ---------- Items ----------
    private void ListItems()
    {
        var suppliers = SupplierDb.GetAll(_conn);
        var items = ItemDb.GetAll(_conn, suppliers);
        if (items.Count == 0) { Console.WriteLine("No items."); return; }

        Console.WriteLine(new string('-', 110));
        Console.WriteLine(
            $"{"ID",-3} {"Category",-10} {"Name",-25} {"Qty",-5} {"ROP",-5} {"Exp",-12} {"Supplier",-18} {"Unit",-10} {"Extra",-25}");
        Console.WriteLine(new string('-', 110));

        foreach (var it in items)
        {
            string exp      = it.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A";
            string supplier = it.Supplier?.SupplierName ?? "N/A";
            string unit     = (it as InventoryItem)?.Unit ?? "";
            string extra    = "";

            if (it is MedicalSupply med)
                extra = $"Lot: {med.LotNumber}  ColdChain: {(med.RequiresColdChain ? "Y" : "N")}";
            else if (it is OfficeSupply off)
                extra = $"Model: {off.Model}";
            else if (!string.IsNullOrWhiteSpace(unit))
                extra = $"Unit: {unit}";

            Console.WriteLine(
                $"{it.ItemId,-3} {it.Category,-10} {it.Name,-25} {it.Quantity,-5} {it.ReorderPoint,-5} {exp,-12} {supplier,-18} {unit,-10} {extra,-25}");
        }

        Console.WriteLine(new string('-', 110));
    }

    private void AddGenericItem()
    {
        var sup = ChooseSupplier();
        if (sup == null) { Console.WriteLine("Add a supplier first."); return; }

        string name   = Input("Item name: ", required: true);
        int qty       = InputInt("Quantity: ");
        int rop       = InputInt("Reorder Point: ");
        DateTime? exp = InputDateNullable("Expiration (YYYY-MM-DD or blank): ");
        string unit   = Input("Unit (e.g., each, box): ");

        var item = new InventoryItem(name, qty, rop, exp, sup, unit);
        ItemDb.Add(_conn, item);
        Console.WriteLine("Added: " + item);
    }

    private void AddMedicalSupply()
    {
        var sup = ChooseSupplier();
        if (sup == null) { Console.WriteLine("Add a supplier first."); return; }

        string name   = Input("Supply name: ", required: true);
        int qty       = InputInt("Quantity: ");
        int rop       = InputInt("Reorder Point: ");
        DateTime? exp = InputDateNullable("Expiration (YYYY-MM-DD or blank): ");
        string unit   = Input("Unit (e.g., box): ");
        bool cold     = Input("Requires cold chain? (y/n): ").Trim().ToLowerInvariant() == "y";
        string lot    = Input("Lot number (optional): ");

        var item = new MedicalSupply(name, qty, rop, exp, sup, unit, cold, lot);
        ItemDb.Add(_conn, item);
        Console.WriteLine("Added: " + item);
    }

    private void AddOfficeSupply()
    {
        var sup = ChooseSupplier();
        if (sup == null) { Console.WriteLine("Add a supplier first."); return; }

        string name   = Input("Supply name: ", required: true);
        int qty       = InputInt("Quantity: ");
        int rop       = InputInt("Reorder Point: ");
        string unit   = Input("Unit (e.g., pack/ream): ");
        string model  = Input("Model (optional): ");

        var item = new OfficeSupply(name, qty, rop, null, sup, unit, model);
        ItemDb.Add(_conn, item);
        Console.WriteLine("Added: " + item);
    }

    private void UpdateItemQuantity()
    {
        int id  = InputInt("ItemId to update: ");
        int qty = InputInt("New quantity: ");
        ItemDb.UpdateQuantity(_conn, id, qty);
        Console.WriteLine("Quantity updated.");
    }

    private void DeleteItem()
    {
        int id = InputInt("ItemId to delete: ");
        ItemDb.Delete(_conn, id);
        Console.WriteLine("Deleted (if existed).");
    }

    // ---------- Input helpers ----------
    private static string Input(string prompt, bool required = false)
    {
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (!required) return s ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(s)) return s!;
            Console.WriteLine("This field is required.");
        }
    }

    private static int InputInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (int.TryParse(s, out int v) && v >= 0) return v;
            Console.WriteLine("Enter a non-negative integer.");
        }
    }

    private static DateTime? InputDateNullable(string prompt)
    {
        Console.Write(prompt);
        string? s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParse(s, out var dt)) return dt;
        Console.WriteLine("Invalid date; leaving blank.");
        return null;
    }
}