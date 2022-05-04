using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.Log(LogLevel.Information, "SubmitCusotmerNumber : {CustomerNumber}", context.Message.CustomerNumber);
            Console.WriteLine("-----------------------------------");

            throw new Exception();

            if (context.Message.CustomerNumber.Contains("Test"))
            {
                if (context.RequestId != null)
                    await context.RespondAsync<OrderSubmitedRejected>(new
                    {

                        OrderId = Guid.NewGuid(),
                        TimeStamp = DateTime.Now,
                        CustomerNumber = context.Message.CustomerNumber,
                        Reason = "Unknown"
                    });
                return;
            }
            if (context.RequestId != null)
                await context.RespondAsync<OrderSubmitionAccepted>(new
                {
                    //  "3d3e0000-198b-14cb-54e7-08da2d290dca"
                    OrderId = new Guid(),
                    TimeStamp = InVar.Timestamp
                });
        }
    }


}
