using Agricon.Core.Model.Entities;
using Agricon.Infrastructure.Repository;

namespace Agricon.Core.Application.Interface.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(string Id);
    }
}
