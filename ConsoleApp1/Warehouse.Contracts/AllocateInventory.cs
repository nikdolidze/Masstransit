using System;

namespace Warehouse.Contracts
{
    public interface AllocateInventory
    {
        Guid AllocationId { get; }  
        string ItemNumber { get; }
        decimal Quantity { get; }

    } 

    public interface AllocationReasonRequested
    {
        Guid AllocationId { get; }
        string Reason { get; }

    }
}
