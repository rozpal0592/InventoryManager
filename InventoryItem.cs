using System;
using System.Globalization;

namespace IMS
{
    public class InventoryItem : InventoryBase
    {
        public string Unit { get; set; }

        public InventoryItem(string name, int quantity, int reorderPoint, DateTime? expiration, Supplier supplier, string unit = "unit")
            : base(name, quantity, reorderPoint, expiration, supplier)
        {
            Category = "Generic";
            Unit = string.IsNullOrWhiteSpace(unit) ? "unit" : unit.Trim();
        }

        public override int CalculateReorderPriority()
        {
            int belowRop = Math.Max(0, ReorderPoint - Quantity);
            int expWeight = IsExpiringSoon(TimeSpan.FromDays(14)) ? 2 : 0;
            return belowRop + expWeight;
        }

        public override string GenerateReport()
        {
            string exp = ExpirationDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "N/A";
            return $@"
Name: {Name}
Category: {Category}
Quantity: {Quantity} {Unit}
Reorder Point: {ReorderPoint}
Expiration: {exp}
Supplier: {Supplier}
Low Stock: {(IsLowStock() ? "YES" : "no")}
Reorder Priority: {CalculateReorderPriority()}";
        }

        public override string ToString() => base.ToString() + $" | Unit: {Unit}";
    }
}
