using MassTransit.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.CurrierActivities
{
    public class PaymentActivity : IActivity<PaymentArguments, PaymentLog>
    {
        public async Task<CompensationResult> Compensate(CompensateContext<PaymentLog> context)
        {
            await Task.Delay(1);
            return  context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<PaymentArguments> context)
        {
            string cardNumber = context.Arguments.CardNumber;
            if(string.IsNullOrEmpty(cardNumber))
                throw new ArgumentNullException(nameof(cardNumber));
            if(cardNumber.StartsWith("5999"))
                throw new InvalidOperationException("Card number was invalid");
            await Task.Delay(30);
            return context.Completed(new
            {AuthoizationCode = "777"}); 
        }
    }
    public interface PaymentLog
    {
        string AuthoizationCode { get; }
    }
    public interface PaymentArguments
    {
        Guid OrderId { get; }
        decimal Amount { get; }
        string CardNumber { get; }
    }
}
