using System;

namespace Sample.Contracts
{
    public interface OrderStatus
    {
        Guid OrderId { get; }
        string CustomerNumber { get; }
        string State { get; }
    }
}
