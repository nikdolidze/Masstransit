using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public OrderController(ILogger<OrderController> logger, IRequestClient<SubmitOrder> submitOrderRequestClient)
        {
            _logger = logger;
            _submitOrderRequestClient = submitOrderRequestClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string customerNumber)
        {
           var (excepdet, rejected) = 
                await _submitOrderRequestClient.GetResponse<OrderSubmitionAccepted,OrderSubmitedRejected>(new
            {

                OrderId = new Guid(),
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
    }
}
