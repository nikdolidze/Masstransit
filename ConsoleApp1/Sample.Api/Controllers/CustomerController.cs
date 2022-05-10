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
    public class CustomerController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public CustomerController(IPublishEndpoint  publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }


        [HttpPut]
        public async Task<IActionResult> Put(Guid id,string customerNumber)
        {

            await _publishEndpoint.Publish<CustomerAccountClosed>(new
            {

                CustomerId = id,
                CustomerNumber = customerNumber
            });
            return Ok();
        }

    }
}
