using System;
namespace Sample.Contracts
{
    public interface FulfilOrder
    {
        Guid OrderId { get; }
        string CustomerNumber { get; }

        string PaymentCardNumber { get; }
    }
}
