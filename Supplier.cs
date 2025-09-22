using System.Collections.Generic;

namespace IMS
{
    /// <summary>Supplier entity (used via aggregation/association by items).</summary>
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public Supplier() { }

        public Supplier(string supplierName, string phone = "", string email = "")
        {
            SupplierName = supplierName ?? string.Empty;
            Phone = phone ?? string.Empty;
            Email = email ?? string.Empty;
        }

        public override string ToString()
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(SupplierName)) parts.Add(SupplierName);
            if (!string.IsNullOrWhiteSpace(Phone)) parts.Add($"📞 {Phone}");
            if (!string.IsNullOrWhiteSpace(Email)) parts.Add($"✉️ {Email}");
            return string.Join(" | ", parts);
        }
    }
}
