// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project
// Description: Defines the InventoryItem class which extends InventoryBase 
// and implements methods for calculating reorder priority and generating reports.

using System;

public abstract class InventoryBase : IReportable
{
    public int       ItemId        { get; protected set; }
    public string    Name          { get; protected set; }
    public string    Category      { get; protected set; }
    public int       Quantity      { get; protected set; }
    public int       ReorderPoint  { get; protected set; }
    public DateTime? ExpirationDate{ get; protected set; }
    public Supplier  Supplier      { get; protected set; }
    public DateTime  CreatedAt     { get; protected set; } = DateTime.UtcNow;
    public DateTime  UpdatedAt     { get; protected set; } = DateTime.UtcNow;

    protected InventoryBase(string category, string name, int qty, int rop, DateTime? exp, Supplier supplier)
    {
        Category = category;
        Name = name;
        Quantity = qty;
        ReorderPoint = rop;
        ExpirationDate = exp;
        Supplier = supplier;
    }

    public void AdjustQuantity(int newQty)
    {
        Quantity = newQty;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLowStock()
    {
        return Quantity <= ReorderPoint;
    }
    public bool IsExpiringSoon(TimeSpan window)
    {
        return ExpirationDate.HasValue && (ExpirationDate.Value - DateTime.Today) <= window;
    }

    public abstract int CalculateReorderPriority();
    public abstract string GenerateReport();
}