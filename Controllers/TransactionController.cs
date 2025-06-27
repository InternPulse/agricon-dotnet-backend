using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Application.Interface.Services;
using Agricon.Core.Dtos;
using Agricon.Core.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Agricon.Controllers
{
    [ApiController]
    [Route("api/v1/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ITransactionRepository _repo;

        public TransactionController(ITransactionService transactionService, ITransactionRepository repo)
        {
            _transactionService = transactionService;
            _repo = repo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction == null)
                return NotFound("Transaction not found");

            return Ok(transaction);
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto model)
        {
            if (!Enum.IsDefined(typeof(TransactionStatus), model.NewStatus))
                return BadRequest("Invalid status");

            await _repo.UpdateStatusAsync(id, model.NewStatus);
            return Ok("Transaction status updated");
        }


        [HttpGet("usertransactions/{bookingId}")]
        public async Task<IActionResult> GetByUserTransactions(int bookingId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _transactionService.GetByUserAsync(bookingId, page, size);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTransactions([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var transactions = await _transactionService.GetAllAsync(page, size);
            return Ok(transactions);
        }
        [HttpGet("{bookingid}/receipt")]
        public async Task<IActionResult> DownloadReceipt(int bookingid)
        {
            var transaction = await _transactionService.GetByBookingIdAsync(bookingid);
            if (transaction == null)
                return NotFound("Transaction not found");

            var pdfBytes = await _transactionService.GenerateTransactionReceiptByBookingIdAsync(bookingid);
            return File(pdfBytes, "application/pdf", $"Booking Receipt.pdf");
        }
    }
}
