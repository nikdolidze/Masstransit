using System;

namespace Sample.Contracts
{
    public interface OrderAccepted
    {
        Guid OrderId { get; }
        DateTime TimeStap { get; }

    }
}
