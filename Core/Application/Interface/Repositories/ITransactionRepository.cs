using Agricon.Core.Model.Entities;
using Agricon.Core.Model.Enums;

namespace Agricon.Core.Application.Interface.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> GetByReferenceAsync(string reference);
        Task SaveChangesAsync();
    }

}
