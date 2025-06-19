using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Model.Entities;
using Agricon.Core.Model.Enums;
using Agricon.Infrastructure.AppContext;
using Microsoft.EntityFrameworkCore;

namespace Agricon.Infrastructure.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AgriconContext _db;

        public TransactionRepository(AgriconContext db)
        {
            _db = db;
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            await _db.Transactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> GetByIdAsync(int id)
        {
            return await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateStatusAsync(int id, PaymentStatus newStatus)
        {
            var transaction = await GetByIdAsync(id);
            if (transaction != null)
            {
                transaction.Status = newStatus;
                transaction.UpdatedAt = DateTime.UtcNow;
                await SaveChangesAsync();
            }
        }

        public async Task<Transaction> GetByReferenceAsync(string reference)
        {
            return await _db.Transactions.FirstOrDefaultAsync(t => t.Reference == reference);
        }

        public async Task<PaginatedResult<Transaction>> GetAllAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _db.Transactions.CountAsync();

            var items = await _db.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Transaction>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResult<Transaction>> GetByUserAsync(int bookingId, int pageNumber, int pageSize)
        {
            var query = _db.Transactions
                .Where(t => t.BookingId == bookingId); 

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Transaction>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }

    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}
