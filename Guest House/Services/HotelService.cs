using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Guest_House.Data;
using Guest_House.Models;
using Guest_House.DTOs.Hotel;

namespace Guest_House.Services
{
    public class HotelService
    {
        private readonly GuestHouseContext _context;

        public HotelService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
        {
            return await _context.Hotels
                .AsNoTracking()
                .Select(h => new HotelDto
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    Address = h.Address,
                    Phone = h.Phone,
                    Email = h.Email,
                    WebsiteUrl = h.WebsiteUrl,
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int hotelId)
        {
            return await _context.Hotels
                .AsNoTracking()
                .Where(h => h.HotelId == hotelId)
                .Select(h => new HotelDto
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    Address = h.Address,
                    Phone = h.Phone,
                    Email = h.Email,
                    WebsiteUrl = h.WebsiteUrl,
                    CreatedAt = h.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<HotelDto> CreateHotelAsync(HotelCreateDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                WebsiteUrl = dto.WebsiteUrl,
                CreatedAt = System.DateTime.Now
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return new HotelDto
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                Phone = hotel.Phone,
                Email = hotel.Email,
                WebsiteUrl = hotel.WebsiteUrl,
                CreatedAt = hotel.CreatedAt
            };
        }

        public async Task<HotelDto?> UpdateHotelAsync(int hotelId, HotelUpdateDto dto)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) return null;

            hotel.Name = dto.Name;
            hotel.Address = dto.Address;
            hotel.Phone = dto.Phone;
            hotel.Email = dto.Email;
            hotel.WebsiteUrl = dto.WebsiteUrl;

            await _context.SaveChangesAsync();

            return new HotelDto
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                Phone = hotel.Phone,
                Email = hotel.Email,
                WebsiteUrl = hotel.WebsiteUrl,
                CreatedAt = hotel.CreatedAt
            };
        }

        public async Task<bool> DeleteHotelAsync(int hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HotelExistsAsync(int hotelId)
        {
            return await _context.Hotels.AnyAsync(h => h.HotelId == hotelId);
        }
    }
}
