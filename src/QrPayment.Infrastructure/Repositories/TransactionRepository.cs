using Microsoft.EntityFrameworkCore;
using QrPayment.Domain.Entities;
using QrPayment.Domain.Interfaces;
using QrPayment.Infrastructure.Persistence;

namespace QrPayment.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
