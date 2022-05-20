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

            //  throw new Exception();

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
                await context.Publish<OrderRejected>(new
                {
                    context.Message.OrderId,
                    TimeStap = context.Message.TimeStapm, 
                    context.Message.CustomerNumber


                });
                return;
            }
            await context.Publish<OrderSubmitted>(new
            {
                context.Message.OrderId,
                TimeStap = context.Message.TimeStapm,
                context.Message.CustomerNumber,
                context.Message.PaymentCardNumber
            });
            if (context.RequestId != null)
                await context.RespondAsync<OrderSubmitionAccepted>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    context.Message.CustomerNumber

                    //OrderId = default(Guid),
                    //Timestamp = default(DateTime),
                    //CustomerNumber = default(string)
                });
        }
    }
}
