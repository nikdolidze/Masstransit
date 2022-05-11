using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.Consumers
{
    public class RoutingSlipEventConsumer : IConsumer<RoutingSlipCompleted>, IConsumer<RoutingSlipActivityCompleted>, IConsumer<RoutingSlipActivityFaulted>
    {
        private readonly ILogger<RoutingSlipEventConsumer> _logger;

        public RoutingSlipEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            if(_logger.IsEnabled(LogLevel.Information))
                _logger.Log(LogLevel.Information,"-------------------------------Routeng slip compleated {TrackingNumber}",context.Message.TrackingNumber);

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Log(LogLevel.Information, "----------------------------------Routeng slip Activity compleated {TrackingNumber} {activityNme}", context.Message.TrackingNumber,context.Message.ActivityName);

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityFaulted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Log(LogLevel.Information, "=====---------------------------Routeng slip RoutingSlipActivityFaulted   compleated {TrackingNumber} {ex}", context.Message.TrackingNumber, context.Message.Duration);

            return Task.CompletedTask;
        }
    }
}
