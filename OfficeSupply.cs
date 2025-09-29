// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project
// Description: Defines the InventoryItem class which extends InventoryBase 
// and implements methods for calculating reorder priority and generating reports.

public class OfficeSupply : InventoryItem
{
    public string Model { get; protected set; }

    public OfficeSupply(string name, int qty, int rop, System.DateTime? exp, Supplier supplier, string unit, string model)
        : base(name, qty, rop, exp, supplier, unit)
    {
        Model = model ?? string.Empty;
        Category = "Office";
    }

    public override int CalculateReorderPriority()
    {
        return base.CalculateReorderPriority();
    }

    public override string GenerateReport()
    {
        return $"{Name} ({Category}) Qty:{Quantity} ROP:{ReorderPoint} Model:{Model}";
    }
}
