using System;
using MassTransit;

namespace Sample.Contracts
{
    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime TimeStapm { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
        MessageData<string> Notes { get; }
    }

}
