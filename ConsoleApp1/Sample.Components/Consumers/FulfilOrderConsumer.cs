using MassTransit;
using MassTransit.Courier;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.Consumers
{
    public class FulfilOrderConsumer : IConsumer<FulfilOrder>
    {
        public async Task Consume(ConsumeContext<FulfilOrder> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddActivity("AllocateInventory", new Uri("queue:allocate-inventory_execute"), new
            {
                ItemNumber = "Item123",
                Quantity = 10.0m
            });

            builder.AddActivity("PaymentActivity", new Uri("queue:payment_execute"), new
            {
                Amount = 99.95,
                CardNumber = context.Message.PaymentCardNumber ?? "5999"
            });

            builder.AddVariable("OrderId", context.Message.OrderId);

            await builder.AddSubscription(context.SourceAddress,
                   MassTransit.Courier.Contracts.RoutingSlipEvents.Faulted
                  | MassTransit.Courier.Contracts.RoutingSlipEvents.Supplemental,
                   MassTransit.Courier.Contracts.RoutingSlipEventContents.None,
                   x => x.Send<OrderFulfilmentFaulted>(new
                   {

                       OrderId = context.Message.OrderId
                   }));


            await builder.AddSubscription(context.SourceAddress, 
                  MassTransit.Courier.Contracts.RoutingSlipEvents.Completed
                 | MassTransit.Courier.Contracts.RoutingSlipEvents.Supplemental,
                  MassTransit.Courier.Contracts.RoutingSlipEventContents.None,
                  x => x.Send<OrderFulfilmentCompleted>(new
                  {

                      OrderId = context.Message.OrderId
                  }));

            var routingSlip = builder.Build();
            await context.Execute(routingSlip);
        }
    }
}
