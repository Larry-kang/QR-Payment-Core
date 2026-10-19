using Microsoft.AspNetCore.Mvc;
using QrPayment.Application.DTOs;
using QrPayment.Domain.Entities;
using QrPayment.Domain.Interfaces;

namespace QrPayment.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ITransactionRepository _repository;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ITransactionRepository repository, ILogger<PaymentController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
            [FromBody] CreatePaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                return BadRequest("Idempotency-Key header is required.");
            }

            var existingTransaction = await _repository.GetByIdempotencyKeyAsync(idempotencyKey);
            if (existingTransaction != null)
            {
                // In a real idempotency implementation, we might return the original result
                // For now, checking existence and returning conflict or just the existing status
                return Conflict($"Transaction with Idempotency-Key '{idempotencyKey}' already exists.");
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                IdempotencyKey = idempotencyKey,
                Amount = request.Amount,
                Currency = "TWD",
                Status = TransactionStatus.Created,
                CreatedAt = DateTime.UtcNow
            };
            
            await _repository.AddAsync(transaction);

            return Ok(new { TransactionId = transaction.Id, Status = transaction.Status });
        }
    }
}
