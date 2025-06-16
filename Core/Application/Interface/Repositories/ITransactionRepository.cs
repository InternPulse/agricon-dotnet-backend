using Agricon.Core.Model.Entities;
using Agricon.Core.Model.Enums;
using Agricon.Infrastructure.Repository;

namespace Agricon.Core.Application.Interface.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> GetByReferenceAsync(string reference);
        Task SaveChangesAsync();

        Task<Transaction> GetByIdAsync(string id); 
        Task UpdateStatusAsync(string id, PaymentStatus newStatus);
        Task<List<Transaction>> GetAllAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<Transaction>> GetByUserAsync(string userId, int pageNumber, int pageSize);

    }


}
