using System;
using System.Collections.Generic;
using System.Globalization;

namespace IMS
{
    public static class ConsoleApp
    {
        private static readonly List<InventoryBase> _items = new();
        private static int _nextId = 1;

        public static void Run()
        {
            SeedSampleData();
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("==== Inventory Management (Week 3 Demo – No DB) ====");
                Console.WriteLine("1) List Items");
                Console.WriteLine("2) Add Generic Item");
                Console.WriteLine("3) Add Medical Supply");
                Console.WriteLine("4) Add Office Supply");
                Console.WriteLine("5) Reports: Low Stock");
                Console.WriteLine("6) Reports: Expiring Soon (30d)");
                Console.WriteLine("0) Exit");
                Console.Write("Select: ");
                var input = Console.ReadLine();
                Console.WriteLine();

                switch (input)
                {
                    case "1": ListItems(); break;
                    case "2": AddGenericInteractive(); break;
                    case "3": AddMedicalInteractive(); break;
                    case "4": AddOfficeInteractive(); break;
                    case "5": ReportLowStock(); break;
                    case "6": ReportExpiringSoon(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice. Try again."); break;
                }
            }
        }

        private static void SeedSampleData()
        {
            if (_items.Count > 0) return;
            var supA = new Supplier("Prime Med Supply", "757-555-1100", "sales@primemed.com");
            var supB = new Supplier("OfficeHub", "757-555-2200", "hello@officehub.com");

            var med = new MedicalSupply(
                name: "Sterile Gloves (M)",
                quantity: 45,
                reorderPoint: 50,
                expiration: DateTime.Today.AddDays(25),
                supplier: supA,
                unit: "box",
                requiresColdChain: false,
                lotNumber: "LOT-GLV-0925");
            AssignId(med);

            var off = new OfficeSupply(
                name: "Printer Paper 8.5x11",
                quantity: 8,
                reorderPoint: 10,
                supplier: supB,
                unit: "ream",
                model: "HP A4 Classic");
            AssignId(off);

            var gen = new InventoryItem(
                name: "Sanitizing Wipes",
                quantity: 12,
                reorderPoint: 10,
                expiration: DateTime.Today.AddDays(120),
                supplier: supA,
                unit: "canister");
            AssignId(gen);
        }

        private static void AssignId(InventoryBase item)
        {
            // simple ID assigner for demo
            item.GetType().GetProperty(nameof(InventoryBase.ItemId))?.SetValue(item, _nextId++);
            _items.Add(item);
        }

        private static void ListItems()
        {
            if (_items.Count == 0)
            {
                Console.WriteLine("No items available.");
                return;
            }

            Console.WriteLine("#   | Category | Name                     | Qty  | ROP | Exp        | Extra");
            Console.WriteLine(new string('-', 80));
            foreach (var it in _items) Console.WriteLine(it.ToString());
        }

        private static void AddGenericInteractive()
        {
            var name = Prompt("Name: ");
            int qty = PromptInt("Quantity: ");
            int rop = PromptInt("Reorder Point: ");
            DateTime? exp = PromptDateNullable("Expiration (YYYY-MM-DD or blank): ");
            var unit = Prompt("Unit (e.g., box, bottle): ");
            var sup = PromptSupplier();

            var item = new InventoryItem(name, qty, rop, exp, sup, unit);
            AssignId(item);
            Console.WriteLine("Added.");
        }

        private static void AddMedicalInteractive()
        {
            var name = Prompt("Name: ");
            int qty = PromptInt("Quantity: ");
            int rop = PromptInt("Reorder Point: ");
            DateTime? exp = PromptDateNullable("Expiration (YYYY-MM-DD or blank): ");
            var unit = Prompt("Unit (e.g., box, bottle): ");
            var cc = Prompt("Requires Cold Chain? (y/n): ").Trim().ToLowerInvariant() == "y";
            var lot = Prompt("Lot Number (optional): ");
            var sup = PromptSupplier();

            var item = new MedicalSupply(name, qty, rop, exp, sup, unit, cc, lot);
            AssignId(item);
            Console.WriteLine("Added.");
        }

        private static void AddOfficeInteractive()
        {
            var name = Prompt("Name: ");
            int qty = PromptInt("Quantity: ");
            int rop = PromptInt("Reorder Point: ");
            var unit = Prompt("Unit (e.g., pack, ream): ");
            var model = Prompt("Model (optional): ");
            var sup = PromptSupplier();

            var item = new OfficeSupply(name, qty, rop, sup, unit, model);
            AssignId(item);
            Console.WriteLine("Added.");
        }

        private static void ReportLowStock()
        {
            Console.WriteLine("-- Low Stock --");
            foreach (var it in _items)
                if (it.IsLowStock())
                    Console.WriteLine($"[LOW] {it.Name} | Qty={it.Quantity} | ROP={it.ReorderPoint} | Priority={it.CalculateReorderPriority()}");
        }

        private static void ReportExpiringSoon()
        {
            Console.WriteLine("-- Expiring within 30 days --");
            foreach (var it in _items)
                if (it.IsExpiringSoon(TimeSpan.FromDays(30)))
                    Console.WriteLine($"[EXP] {it.Name} | Exp={it.ExpirationDate:yyyy-MM-dd} | Priority={it.CalculateReorderPriority()}");
        }

        // Helpers
        private static string Prompt(string label) { Console.Write(label); return Console.ReadLine() ?? string.Empty; }

        private static int PromptInt(string label)
        {
            while (true)
            {
                Console.Write(label);
                if (int.TryParse(Console.ReadLine(), out int value) && value >= 0) return value;
                Console.WriteLine("Please enter a non-negative integer.");
            }
        }

        private static DateTime? PromptDateNullable(string label)
        {
            Console.Write(label);
            var s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateTime.TryParseExact(s!.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dt)) return dt;
            Console.WriteLine("Invalid date; leaving blank.");
            return null;
        }

        private static Supplier PromptSupplier()
        {
            var name = Prompt("Supplier Name: ");
            var phone = Prompt("Phone (optional): ");
            var email = Prompt("Email (optional): ");
            return new Supplier(name, phone, email);
        }
    }
}
