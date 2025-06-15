using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Model.Entities;
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

        public async Task<Transaction> GetByReferenceAsync(string reference)
        {
            return await _db.Transactions.FirstOrDefaultAsync(t => t.Reference == reference);
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }

}
