using System;

namespace Sample.Contracts
{
    public interface OrderSubmitionAccepted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }

        string CustomerNumber { get; }
    }
    public interface OrderSubmitedRejected
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }
        string CustomerNumber { get; }
        string Reason { get; }

    }
}
