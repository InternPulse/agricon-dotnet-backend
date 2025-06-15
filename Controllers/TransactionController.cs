using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Dtos;
using Agricon.Core.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Agricon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _repo;

        public TransactionController(ITransactionRepository repo)
        {
            _repo = repo;
        }

        // ✅ Get transaction by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction == null)
                return NotFound("Transaction not found");

            return Ok(transaction);
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateStatusDto model)
        {
            if (!Enum.IsDefined(typeof(PaymentStatus), model.NewStatus))
                return BadRequest("Invalid status");

            await _repo.UpdateStatusAsync(id, model.NewStatus);
            return Ok("Transaction status updated");
        }
    }
}
