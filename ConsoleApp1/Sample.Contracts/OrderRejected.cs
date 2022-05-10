using System;

namespace Sample.Contracts
{
    public interface OrderRejected
    {
        Guid OrderId { get; }
        DateTime TimeStap { get; }
        string CustomerNumber { get; }

    }
}
