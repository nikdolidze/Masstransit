using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Components.Consumers;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {


        private readonly ILogger<OrderController> _logger;
        readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IRequestClient<CheckOrder> _checkOrderClient;
        private readonly IPublishEndpoint _publish;

        public OrderController(ILogger<OrderController> logger, IRequestClient<SubmitOrder> submitOrderRequestClient, ISendEndpointProvider sendEndpointProvider
            , IRequestClient<CheckOrder> checkOrder,IPublishEndpoint publish)
        {
            _logger = logger;
            _submitOrderRequestClient = submitOrderRequestClient;
            _sendEndpointProvider = sendEndpointProvider;
            _checkOrderClient = checkOrder;
            _publish = publish;
        }


        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var (status, notFount) = await _checkOrderClient.GetResponse<OrderStatus, OrderNotFound>(new
            {
                OrderId = id
            });

            if (status.IsCompletedSuccessfully)
            {
                var reposne = await status;
                return Ok(reposne.Message);
            }

         var response =    await notFount;


            return NotFound(response.Message) ;
        }



        [HttpPost]
        public async Task<IActionResult> Post(string customerNumber)
        {
            var (excepdet, rejected) =
                 await _submitOrderRequestClient.GetResponse<OrderSubmitionAccepted, OrderSubmitedRejected>(new
                 {

                     OrderId = Guid.NewGuid(),
                     TimeStapm = InVar.Timestamp,
                     CustomerNumber = customerNumber
                 });
            if (excepdet.IsCompletedSuccessfully)
            {
                var response = await excepdet;
                return Ok(response);
            }
            else
            {
                var response = await rejected;
                return BadRequest(response);
            }

        }


        [HttpPatch]
        public async Task<IActionResult> HttpPatch(Guid id)
        {

            await _publish.Publish<OrderAccepted>(new
            {

                OrderId = id,
                TimeStap = InVar.Timestamp,

            });
            return Accepted();
        }



        [HttpPut]
        public async Task<IActionResult> Put(string customerNumber)
        {
            var a = KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>();

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:submit-order"));
            await endpoint.Send<SubmitOrder>(new
            {

                OrderId = new Guid().ToNewId(),
                TimeStapm = InVar.Timestamp,
                CustomerNumber = customerNumber
            });
            return Accepted();
        }
    }
}
