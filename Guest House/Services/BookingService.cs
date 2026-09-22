using Microsoft.EntityFrameworkCore;
using Guest_House.Data;
using Guest_House.Models;

namespace Guest_House.Services
{
    public class BookingService
    {
        private readonly GuestHouseContext _context;

        public BookingService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<int>> GetAvailableRoomIdsAsync(int hotelId, DateTime checkIn, DateTime checkOut)
        {
            var bookedRoomIds = _context.BookingRooms
                .Where(br => br.Booking.BookingStatus == "confirmed" || br.Booking.BookingStatus == "checked_in")
                .Where(br => br.Booking.CheckInDate < checkOut && br.Booking.ExpectedCheckout > checkIn)
                .Select(br => br.RoomId);

            return await _context.Rooms
                .Where(r => r.HotelId == hotelId && !bookedRoomIds.Contains(r.RoomId))
                .Select(r => r.RoomId)
                .ToListAsync();
        }

        public async Task<int> CreateBookingAsync(Booking booking, List<BookingRoom> rooms)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync(); // booking.BookingId is now populated

                foreach (var room in rooms)
                {
                    room.BookingId = booking.BookingId;
                    _context.BookingRooms.Add(room);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return booking.BookingId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.BookingRooms)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string newStatus)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.BookingStatus = newStatus;
            booking.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}