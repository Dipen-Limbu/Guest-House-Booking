using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Stay;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;
using StayEntity = Guest_House.Models.Stay;

namespace Guest_House.Services.Stay
{
    public class StayService : IStayService
    {
        private readonly GuestHouseContext _context;

        public StayService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<StayResponseDto> CheckInAsync(CheckInDto dto, CancellationToken cancellationToken = default)
        {
            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Stay)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(b => b.BookingId == dto.BookingId, cancellationToken)
                ?? throw new KeyNotFoundException($"Booking with ID {dto.BookingId} was not found.");

            if (booking.BookingStatus.ToLower() == "cancelled")
                throw new InvalidOperationException("Cannot perform check-in on a cancelled booking.");

            // Check if stay already exists or is checked in
            var existingStay = await _context.Stays
                .FirstOrDefaultAsync(s => s.BookingId == dto.BookingId, cancellationToken);

            if (existingStay != null && existingStay.StayStatus.ToLower() == "active")
                throw new InvalidOperationException($"Booking #{dto.BookingId} is already checked in (Stay ID {existingStay.StayId}).");

            if (existingStay != null && existingStay.StayStatus.ToLower() == "completed")
                throw new InvalidOperationException($"Booking #{dto.BookingId} has already completed its stay.");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var now = DateTime.Now;
                var stay = existingStay ?? new StayEntity
                {
                    BookingId = dto.BookingId,
                    CreatedAt = now
                };

                stay.ActualCheckin = now;
                stay.StayStatus = "active";
                stay.Notes = dto.Notes?.Trim();

                if (existingStay == null)
                    _context.Stays.Add(stay);

                booking.BookingStatus = "checked_in";
                booking.UpdatedAt = now;

                // Update associated rooms to occupied
                foreach (var bookingRoom in booking.BookingRooms)
                {
                    if (bookingRoom.Room != null)
                    {
                        bookingRoom.Room.Status = "occupied";
                        bookingRoom.Room.UpdatedAt = now;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return await GetByIdAsync(stay.StayId, cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<StayResponseDto> CheckOutAsync(int stayId, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays
                .Include(s => s.Booking)
                    .ThenInclude(b => b.BookingRooms)
                        .ThenInclude(br => br.Room)
                .FirstOrDefaultAsync(s => s.StayId == stayId, cancellationToken)
                ?? throw new KeyNotFoundException($"Stay record with ID {stayId} was not found.");

            if (stay.StayStatus.ToLower() == "completed")
                throw new InvalidOperationException($"Stay #{stayId} has already been checked out.");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var now = DateTime.Now;
                stay.ActualCheckout = now;
                stay.StayStatus = "completed";

                if (stay.Booking != null)
                {
                    stay.Booking.BookingStatus = "checked_out";
                    stay.Booking.UpdatedAt = now;

                    foreach (var bookingRoom in stay.Booking.BookingRooms)
                    {
                        if (bookingRoom.Room != null)
                        {
                            bookingRoom.Room.Status = "cleaning";
                            bookingRoom.Room.UpdatedAt = now;
                        }
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return await GetByIdAsync(stayId, cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<List<StayResponseDto>> GetAllAsync(int? bookingId, string? stayStatus, CancellationToken cancellationToken = default)
        {
            var query = _context.Stays
                .AsNoTracking()
                .Include(s => s.Booking)
                    .ThenInclude(b => b.Guest)
                .Include(s => s.Booking)
                    .ThenInclude(b => b.BookingRooms)
                        .ThenInclude(br => br.Room)
                            .ThenInclude(r => r.Category)
                .AsQueryable();

            if (bookingId.HasValue)
                query = query.Where(s => s.BookingId == bookingId.Value);

            if (!string.IsNullOrWhiteSpace(stayStatus))
                query = query.Where(s => s.StayStatus.ToLower() == stayStatus.Trim().ToLower());

            var stays = await query
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);

            return stays.Select(ToDto).ToList();
        }

        public async Task<StayResponseDto> GetByIdAsync(int stayId, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays
                .AsNoTracking()
                .Include(s => s.Booking)
                    .ThenInclude(b => b.Guest)
                .Include(s => s.Booking)
                    .ThenInclude(b => b.BookingRooms)
                        .ThenInclude(br => br.Room)
                            .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(s => s.StayId == stayId, cancellationToken)
                ?? throw new KeyNotFoundException($"Stay record with ID {stayId} was not found.");

            return ToDto(stay);
        }

        public async Task<List<StayResponseDto>> GetActiveStaysAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllAsync(null, "active", cancellationToken);
        }

        public async Task<StayResponseDto> UpdateAsync(int stayId, UpdateStayDto dto, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays
                .Include(s => s.Booking)
                    .ThenInclude(b => b.Guest)
                .Include(s => s.Booking)
                    .ThenInclude(b => b.BookingRooms)
                        .ThenInclude(br => br.Room)
                            .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(s => s.StayId == stayId, cancellationToken)
                ?? throw new KeyNotFoundException($"Stay record with ID {stayId} was not found.");

            if (!string.IsNullOrWhiteSpace(dto.StayStatus))
            {
                var validStatuses = new[] { "active", "completed", "cancelled" };
                var status = dto.StayStatus.Trim().ToLower();
                if (!validStatuses.Contains(status))
                    throw new ArgumentException($"Invalid stay status '{dto.StayStatus}'. Allowed: {string.Join(", ", validStatuses)}.");
                stay.StayStatus = status;
            }

            if (dto.Notes != null)
                stay.Notes = dto.Notes.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(stayId, cancellationToken);
        }

        private static StayResponseDto ToDto(StayEntity s)
        {
            var roomDtos = s.Booking?.BookingRooms?.Select(br => new StayRoomDetailsDto
            {
                RoomId = br.RoomId,
                RoomNumber = br.Room?.RoomNumber ?? string.Empty,
                CategoryName = br.Room?.Category?.CategoryName ?? string.Empty,
                RoomStatus = br.Room?.Status ?? string.Empty
            }).ToList() ?? new List<StayRoomDetailsDto>();

            return new StayResponseDto
            {
                StayId = s.StayId,
                BookingId = s.BookingId,
                BookingReference = s.Booking?.BookingReference ?? string.Empty,
                GuestId = s.Booking?.GuestId ?? 0,
                GuestName = s.Booking?.Guest?.FullName ?? string.Empty,
                GuestPhone = s.Booking?.Guest?.Phone ?? string.Empty,
                ActualCheckin = s.ActualCheckin,
                ActualCheckout = s.ActualCheckout,
                StayStatus = s.StayStatus,
                Notes = s.Notes,
                CreatedAt = s.CreatedAt,
                Rooms = roomDtos
            };
        }
    }
}
