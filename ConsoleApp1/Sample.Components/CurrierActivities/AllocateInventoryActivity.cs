using MassTransit;
using MassTransit.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Contracts;

namespace Sample.Components.CurrierActivities
{
    public class AllocateInventoryActivity : IActivity<AllocateInventoryArguments, AllocateInventoryLog>
    {
        readonly IRequestClient<AllocateInventory> _client;

        public AllocateInventoryActivity(IRequestClient<AllocateInventory> client)
        {
            _client = client;
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
        {
            await context.Publish<AllocationReasonRequested>(new
            {

                AllocationId = context.Log.AllocationId,
                Reason = "Order faulted"
            });
            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateInventoryArguments> context)
        {
            var itemNumber = context.Arguments.ItemNumber;
            if(string.IsNullOrWhiteSpace(itemNumber))
                throw new ArgumentNullException(nameof(itemNumber));
            var quantity = context.Arguments.Quantity;
            if(quantity<0)
                throw new ArgumentOutOfRangeException(nameof(quantity));    
            var orderId = context.Arguments.OrderId;
            var allocationId = NewId.NextGuid();

            var response = await _client.GetResponse<InventoryAllocated>(new
            {

                AllocationId = allocationId,
                ItemNumber = itemNumber,
                Quantity = quantity
            });

            return context.Completed(new
            {
                AllocationId = allocationId
            });
        }
    }

    public interface AllocateInventoryArguments
    {
        Guid OrderId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }
    public interface AllocateInventoryLog
    {
        Guid AllocationId { get; }

    }
}
