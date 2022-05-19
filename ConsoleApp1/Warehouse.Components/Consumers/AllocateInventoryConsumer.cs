using MassTransit;
using System;
using System.Threading.Tasks;
using Warehouse.Contracts;

namespace Warehouse.Components.Consumers
{
    public class AllocateInventoryConsumer : IConsumer<AllocateInventory>
    {
        public async Task Consume(ConsumeContext<AllocateInventory> context)
        {
            await Task.Delay(5);

            await context.Publish<AllocatoinCreated>(new
            {

                AllocationId = context.Message.AllocationId,
                HolDuration = 5000,
            }) ;

            await context.RespondAsync<InventoryAllocated>(new
            {
                context.Message.AllocationId,
                context.Message.ItemNumber,
                context.Message.Quantity
            });
        }
    }
}
