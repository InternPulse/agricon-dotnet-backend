using Agricon.Core.Application.Interface.Services;
using Agricon.Core.Application.Services;
using Agricon.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Agricon.Controllers
{
    [ApiController]
    [Route("api/v1/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly ITransactionService _service;

        public PaymentController(ITransactionService service)
        {
            _service = service;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment(PaymentRequestDto request)
        {
            var result = await _service.InitiatePaymentAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("verify/{reference}")]
        public async Task<IActionResult> Verify(string reference)
        {
            var result = await _service.VerifyPaymentAsync(reference);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

}
