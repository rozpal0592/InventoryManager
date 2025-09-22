using System;

namespace IMS
{
    public class OfficeSupply : InventoryItem
    {
        public string Model { get; set; }

        public OfficeSupply(string name, int quantity, int reorderPoint, Supplier supplier,
                            string unit = "unit", string model = "")
            : base(name, quantity, reorderPoint, expiration: null, supplier, unit)
        {
            Category = "Office";
            Model = model ?? string.Empty;
        }

        public override int CalculateReorderPriority()
            => Math.Max(0, ReorderPoint - Quantity); // office supplies typically have no expiration

        public override string GenerateReport()
            => base.GenerateReport()
               + $"\nModel: {(string.IsNullOrWhiteSpace(Model) ? "N/A" : Model)}";

        public override string ToString()
            => base.ToString()
               + $" | Model: {(string.IsNullOrWhiteSpace(Model) ? "-" : Model)}";
    }
}
