using System;

namespace Warehouse.Contracts
{
    public interface AllocationReasonRequested
    {
        Guid AllocationId { get; }
        string Reason { get; }

    }
}
