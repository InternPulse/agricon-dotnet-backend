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

        Task<Transaction> GetByIdAsync(int id); 
        Task<Transaction> GetByBookingIdAsync(int bookingid); 
        Task UpdateStatusAsync(int id, TransactionStatus newStatus);
        Task<PaginatedResult<PaymentDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<PaymentDto>> GetByUserAsync(int userId, int pageNumber, int pageSize);

    }


}
