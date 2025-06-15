using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Agricon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] PaymentRequestDto request)
        {
            var result = await _service.InitiatePaymentAsync(request);
            return result.Status == "failed" ? BadRequest(result) : Ok(result);
        }

        [HttpGet("verify/{reference}")]
        public async Task<IActionResult> Verify(string reference)
        {
            var result = await _service.VerifyPaymentAsync(reference);
            return result.Status == "failed" ? BadRequest(result) : Ok(result);
        }
    }

}
