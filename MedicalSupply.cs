using System;

namespace IMS
{
    public class MedicalSupply : InventoryItem
    {
        public bool RequiresColdChain { get; set; }
        public string LotNumber { get; set; }

        public MedicalSupply(string name, int quantity, int reorderPoint, DateTime? expiration, Supplier supplier,
                             string unit = "unit", bool requiresColdChain = false, string lotNumber = "")
            : base(name, quantity, reorderPoint, expiration, supplier, unit)
        {
            Category = "Medical";
            RequiresColdChain = requiresColdChain;
            LotNumber = lotNumber ?? string.Empty;
        }

        public override int CalculateReorderPriority()
        {
            int baseScore = base.CalculateReorderPriority();
            if (IsExpiringSoon(TimeSpan.FromDays(30))) baseScore += 2;
            if (RequiresColdChain) baseScore += 1;
            return baseScore;
        }

        public override string GenerateReport()
            => base.GenerateReport()
               + $"\nLot Number: {(string.IsNullOrWhiteSpace(LotNumber) ? "N/A" : LotNumber)}"
               + $"\nCold Chain: {(RequiresColdChain ? "YES" : "no")}";

        public override string ToString()
            => base.ToString()
               + $" | Lot: {(string.IsNullOrWhiteSpace(LotNumber) ? "-" : LotNumber)}"
               + $" | ColdChain: {(RequiresColdChain ? "Y" : "N")}";
    }
}
