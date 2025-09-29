// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project
// Description: Defines the InventoryItem class which extends InventoryBase 
// and implements methods for calculating reorder priority and generating reports.

public class Supplier
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    public Supplier(string supplierName, string phone, string email)
    {
        SupplierName = supplierName ?? string.Empty;
        Phone = phone ?? string.Empty;
        Email = email ?? string.Empty;
    }

    public override string ToString()
    {
        return $"[{SupplierId}] {SupplierName} | Phone: {Phone}, Email: {Email}";
    }
}