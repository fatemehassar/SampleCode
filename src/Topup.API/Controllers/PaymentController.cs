using MediatR;
using Microsoft.AspNetCore.Mvc;
using Topup.Application.Features.Purchase.Command;

namespace Topup.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase(
            PurchaseCommand command)
        {
            var result =
                await _mediator.Send(command);

            return Ok(result);
        }
    }
}
