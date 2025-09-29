// Rozz Pallera
// Date: 28 Sept 25
// SDC320 Project
// Description: Defines the InventoryItem class which extends InventoryBase 
// and implements methods for calculating reorder priority and generating reports.

using System;

public class MedicalSupply : InventoryItem
{
    public bool   RequiresColdChain { get; protected set; }
    public string LotNumber         { get; protected set; }

    public MedicalSupply(string name, int qty, int rop, DateTime? exp, Supplier supplier, string unit,
                         bool requiresColdChain, string lot)
        : base(name, qty, rop, exp, supplier, unit)
    {
        RequiresColdChain = requiresColdChain;
        LotNumber = lot ?? string.Empty;
        Category = "Medical";
    }

    public override int CalculateReorderPriority()
    {
        int score = base.CalculateReorderPriority();
        if (RequiresColdChain)
        {
            score += 1;
        }
        return score;
    }
    public override string GenerateReport()
    {
        string cold = RequiresColdChain ? "Y" : "N";
        return $"{Name} ({Category}) Qty:{Quantity} ROP:{ReorderPoint} Lot:{LotNumber} ColdChain:{(cold)}";
    }
}