using Agricon.Core.Application.Interface.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Agricon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IPaymentService _service;

        public WebhookController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("paystack")]
        public async Task<IActionResult> ReceiveWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            dynamic payload = JsonConvert.DeserializeObject(body);

            string eventType = payload.@event;
            string reference = payload.data.reference;
            string status = payload.data.status;

            if (eventType == "charge.success" && status == "success")
            {
                await _service.VerifyPaymentAsync(reference);
            }

            return Ok();
        }
    }

}
