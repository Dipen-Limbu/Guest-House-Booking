using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Hotel;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;
using HotelEntity = Guest_House.Models.Hotel;

namespace Guest_House.Services.Hotel
{
    public class HotelService : IHotelService
    {
        private readonly GuestHouseContext _context;

        public HotelService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<List<HotelResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var hotels = await _context.Hotels
                .AsNoTracking()
                .Include(h => h.Rooms)
                .Include(h => h.StaffUsers)
                .OrderBy(h => h.Name)
                .ToListAsync(cancellationToken);

            return hotels.Select(ToDto).ToList();
        }

        public async Task<HotelResponseDto> GetByIdAsync(int hotelId, CancellationToken cancellationToken = default)
        {
            var hotel = await _context.Hotels
                .AsNoTracking()
                .Include(h => h.Rooms)
                .Include(h => h.StaffUsers)
                .FirstOrDefaultAsync(h => h.HotelId == hotelId, cancellationToken)
                ?? throw new KeyNotFoundException($"Hotel with ID {hotelId} was not found.");

            return ToDto(hotel);
        }

        public async Task<HotelResponseDto> CreateAsync(CreateHotelDto dto, CancellationToken cancellationToken = default)
        {
            var name = dto.Name.Trim();
            var duplicate = await _context.Hotels.AnyAsync(h => h.Name.ToLower() == name.ToLower(), cancellationToken);
            if (duplicate)
                throw new InvalidOperationException($"A hotel with name '{name}' already exists.");

            var hotel = new HotelEntity
            {
                Name = name,
                Address = dto.Address.Trim(),
                Phone = dto.Phone.Trim(),
                Email = dto.Email?.Trim(),
                WebsiteUrl = dto.WebsiteUrl?.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(hotel.HotelId, cancellationToken);
        }

        public async Task<HotelResponseDto> UpdateAsync(int hotelId, UpdateHotelDto dto, CancellationToken cancellationToken = default)
        {
            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.HotelId == hotelId, cancellationToken)
                ?? throw new KeyNotFoundException($"Hotel with ID {hotelId} was not found.");

            var name = dto.Name.Trim();
            var duplicate = await _context.Hotels
                .AnyAsync(h => h.Name.ToLower() == name.ToLower() && h.HotelId != hotelId, cancellationToken);
            if (duplicate)
                throw new InvalidOperationException($"Another hotel with name '{name}' already exists.");

            hotel.Name = name;
            hotel.Address = dto.Address.Trim();
            hotel.Phone = dto.Phone.Trim();
            hotel.Email = dto.Email?.Trim();
            hotel.WebsiteUrl = dto.WebsiteUrl?.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(hotelId, cancellationToken);
        }

        public async Task DeleteAsync(int hotelId, CancellationToken cancellationToken = default)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.StaffUsers)
                .Include(h => h.HotelExpenses)
                .FirstOrDefaultAsync(h => h.HotelId == hotelId, cancellationToken)
                ?? throw new KeyNotFoundException($"Hotel with ID {hotelId} was not found.");

            if (hotel.Rooms.Any() || hotel.StaffUsers.Any() || hotel.HotelExpenses.Any())
            {
                throw new InvalidOperationException("Cannot delete hotel that has associated rooms, staff users, or expense records.");
            }

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static HotelResponseDto ToDto(HotelEntity h)
        {
            return new HotelResponseDto
            {
                HotelId = h.HotelId,
                Name = h.Name,
                Address = h.Address,
                Phone = h.Phone,
                Email = h.Email,
                WebsiteUrl = h.WebsiteUrl,
                CreatedAt = h.CreatedAt,
                TotalRooms = h.Rooms?.Count ?? 0,
                TotalStaff = h.StaffUsers?.Count ?? 0
            };
        }
    }
}
