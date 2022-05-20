using Automatonymous;
using GreenPipes;
using MassTransit;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.StateMachines.OrderStateMachineActivities
{
    public class AcceptOrderActivity : Activity<OrderState, OrderAccepted>

    {
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, Behavior<OrderState, OrderAccepted> next)
        {
            Console.WriteLine("Hello world. Order is ----------------------------------{0}",context.Data.OrderId);

            var consumerContext = context.GetPayload<ConsumeContext>();
            var sendEdnpont = await consumerContext.GetSendEndpoint(new Uri("Exchange:fulfil-order"));
            await sendEdnpont.Send<FulfilOrder>(new
            {

                OrderId = context.Data.OrderId,
                PaymentCardNumber = context.Instance.PaymentCartNumber,
                CustomerNumber = context.Instance.CustomerNumber
            }) ;
            // do something later
           await next.Execute(context).ConfigureAwait(false); 
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, Behavior<OrderState, OrderAccepted> next) where TException : Exception
        {
           return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("accept-order");

          //  context.Add("name", "accept-order");
        }
    }
}
