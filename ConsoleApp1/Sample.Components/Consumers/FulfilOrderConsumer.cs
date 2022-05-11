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

            builder.AddVariable("OrderId", context.Message.OrderId);
            var routingSlip = builder.Build();
            await context.Execute(routingSlip);
        }
    }
}
