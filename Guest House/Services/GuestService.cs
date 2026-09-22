using Microsoft.EntityFrameworkCore;
using Guest_House.Data;
using Guest_House.Models;

namespace Guest_House.Services
{
    public class GuestService
    {
        private readonly GuestHouseContext _context;

        public GuestService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Guest>> GetAllGuestsAsync()
        {
            return await _context.Guests.ToListAsync();
        }

        public async Task<Guest?> GetGuestByIdAsync(int guestId)
        {
            return await _context.Guests.FindAsync(guestId);
        }

        public async Task<Guest?> GetGuestByPhoneAsync(string phone)
        {
            return await _context.Guests.FirstOrDefaultAsync(g => g.Phone == phone);
        }

        public async Task<int> CreateGuestAsync(Guest guest)
        {
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
            return guest.GuestId;
        }

        public async Task<bool> UpdateGuestAsync(Guest guest)
        {
            var existing = await _context.Guests.FindAsync(guest.GuestId);
            if (existing == null) return false;

            existing.FullName = guest.FullName;
            existing.Phone = guest.Phone;
            existing.Email = guest.Email;
            existing.Address = guest.Address;
            existing.IdProofType = guest.IdProofType;
            existing.IdProofNumber = guest.IdProofNumber;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}