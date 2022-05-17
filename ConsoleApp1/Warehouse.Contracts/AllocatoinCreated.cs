using System;

namespace Warehouse.Contracts
{
    public interface AllocatoinCreated
    {
        Guid AllocationId { get; }  
        TimeSpan HolDuration { get; }    


    }
}
