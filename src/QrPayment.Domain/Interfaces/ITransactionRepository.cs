using System;
using System.Threading.Tasks;
using QrPayment.Domain.Entities;

namespace QrPayment.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);
        Task AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
    }
}
