// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project
// Description: Defines the InventoryItem class which extends InventoryBase 
// and implements methods for calculating reorder priority and generating reports.

using System;

public class InventoryItem : InventoryBase
{
    public string Unit { get; protected set; }

    public InventoryItem(string name, int qty, int rop, DateTime? exp, Supplier supplier, string unit)
        : base("Generic", name, qty, rop, exp, supplier)
    {
        Unit = unit ?? string.Empty;
    }

    public override int CalculateReorderPriority()
        => (IsLowStock() ? 2 : 0) + (IsExpiringSoon(TimeSpan.FromDays(30)) ? 1 : 0);

    public override string GenerateReport()
    {
        return $"{Name} ({Category}) Qty:{Quantity} ROP:{ReorderPoint} Unit:{Unit}";
    }
}