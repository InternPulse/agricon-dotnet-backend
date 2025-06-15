using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Model.Entities;
using Agricon.Infrastructure.AppContext;
using Microsoft.EntityFrameworkCore;
using System;

namespace Agricon.Infrastructure.Repository
{
    public class BookingRespository : IBookingRepository
    {
        private readonly AgriconContext _context;

        public BookingRespository(AgriconContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(string Id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(p => p.Id == Id);
        }

    }
}
