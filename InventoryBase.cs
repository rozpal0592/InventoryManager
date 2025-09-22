using System;
using System.Globalization;

namespace IMS
{
    public abstract class InventoryBase : IReportable
    {
        public int ItemId { get; protected set; }
        public string Name { get; set; }
        public string Category { get; protected set; } = "Generic";
        public int Quantity { get; set; }
        public int ReorderPoint { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        protected InventoryBase(string name, int quantity, int reorderPoint, DateTime? expiration, Supplier supplier)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative");
            if (reorderPoint < 0) throw new ArgumentOutOfRangeException(nameof(reorderPoint), "ReorderPoint cannot be negative");

            Name = name.Trim();
            Quantity = quantity;
            ReorderPoint = reorderPoint;
            ExpirationDate = expiration;
            Supplier = supplier ?? new Supplier();

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public abstract int CalculateReorderPriority();
        public abstract string GenerateReport();

        public virtual bool IsLowStock() => Quantity <= ReorderPoint;

        public virtual bool IsExpiringSoon(TimeSpan window)
        {
            if (ExpirationDate is null) return false;
            var today = DateTime.Today;
            return ExpirationDate.Value.Date <= today + window;
        }

        protected void TouchUpdatedAt() => UpdatedAt = DateTime.UtcNow;

        public override string ToString()
        {
            string exp = ExpirationDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "N/A";
            return $"#{ItemId,3} | {Category,-7} | {Name,-24} | Qty: {Quantity,4} | ROP: {ReorderPoint,3} | Exp: {exp,-10}";
        }
    }
}
