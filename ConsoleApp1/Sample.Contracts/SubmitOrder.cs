using System;

namespace Sample.Contracts
{
    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime TimeStapm { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
    }
}
