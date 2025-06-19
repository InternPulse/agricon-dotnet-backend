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

        public async Task<Booking?> GetByIdAsync(int Id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(p => p.Id == Id);
        }
        public async Task<bool> UpdatePaidAsync(int bookingId, bool isPaid)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return false;
            }

            booking.Paid = isPaid;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
