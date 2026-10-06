using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Hotel;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;
using HotelExpenseEntity = Guest_House.Models.HotelExpense;

namespace Guest_House.Services.Hotel
{
    public class HotelExpenseService : IHotelExpenseService
    {
        public static readonly string[] AllowedPaymentMethods = { "cash", "card", "bank_transfer", "other" };

        private readonly GuestHouseContext _context;

        public HotelExpenseService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<List<HotelExpenseResponseDto>> GetAllAsync(
            int? hotelId,
            string? category,
            DateTime? startDate,
            DateTime? endDate,
            CancellationToken cancellationToken = default)
        {
            var query = _context.HotelExpenses
                .AsNoTracking()
                .Include(e => e.Hotel)
                .AsQueryable();

            if (hotelId.HasValue)
                query = query.Where(e => e.HotelId == hotelId.Value);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(e => e.ExpenseCategory.ToLower() == category.Trim().ToLower());

            if (startDate.HasValue)
                query = query.Where(e => e.ExpenseDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ExpenseDate <= endDate.Value);

            var expenses = await query
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync(cancellationToken);

            return expenses.Select(ToDto).ToList();
        }

        public async Task<HotelExpenseResponseDto> GetByIdAsync(int expenseId, CancellationToken cancellationToken = default)
        {
            var expense = await _context.HotelExpenses
                .AsNoTracking()
                .Include(e => e.Hotel)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId, cancellationToken)
                ?? throw new KeyNotFoundException($"Hotel expense record with ID {expenseId} was not found.");

            return ToDto(expense);
        }

        public async Task<HotelExpenseSummaryDto> GetSummaryAsync(
            int hotelId,
            DateTime? startDate,
            DateTime? endDate,
            CancellationToken cancellationToken = default)
        {
            var hotel = await _context.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HotelId == hotelId, cancellationToken)
                ?? throw new KeyNotFoundException($"Hotel with ID {hotelId} was not found.");

            var expensesList = await GetAllAsync(hotelId, null, startDate, endDate, cancellationToken);
            var totalExpenses = expensesList.Sum(e => e.Amount);

            return new HotelExpenseSummaryDto
            {
                HotelId = hotelId,
                TotalExpenses = totalExpenses,
                ExpenseCount = expensesList.Count,
                Expenses = expensesList
            };
        }

        public async Task<HotelExpenseResponseDto> CreateAsync(CreateHotelExpenseDto dto, CancellationToken cancellationToken = default)
        {
            var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId, cancellationToken);
            if (!hotelExists)
                throw new KeyNotFoundException($"Hotel with ID {dto.HotelId} was not found.");

            string? paymentMethod = null;
            if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                paymentMethod = dto.PaymentMethod.Trim().ToLower();
                if (!AllowedPaymentMethods.Contains(paymentMethod))
                    throw new ArgumentException($"Invalid payment method '{dto.PaymentMethod}'. Allowed: {string.Join(", ", AllowedPaymentMethods)}.");
            }

            var expense = new HotelExpenseEntity
            {
                HotelId = dto.HotelId,
                ExpenseCategory = dto.ExpenseCategory.Trim(),
                Description = dto.Description?.Trim(),
                Amount = dto.Amount,
                PaymentMethod = paymentMethod,
                ExpenseDate = dto.ExpenseDate ?? DateTime.Now,
                ReceiptUrl = dto.ReceiptUrl?.Trim()
            };

            _context.HotelExpenses.Add(expense);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(expense.ExpenseId, cancellationToken);
        }

        public async Task<HotelExpenseResponseDto> UpdateAsync(int expenseId, UpdateHotelExpenseDto dto, CancellationToken cancellationToken = default)
        {
            var expense = await _context.HotelExpenses
                .Include(e => e.Hotel)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId, cancellationToken)
                ?? throw new KeyNotFoundException($"Hotel expense record with ID {expenseId} was not found.");

            if (!string.IsNullOrWhiteSpace(dto.ExpenseCategory))
                expense.ExpenseCategory = dto.ExpenseCategory.Trim();

            if (dto.Description != null)
                expense.Description = dto.Description.Trim();

            if (dto.Amount.HasValue)
            {
                if (dto.Amount.Value <= 0)
                    throw new ArgumentException("Expense amount must be greater than zero.");
                expense.Amount = dto.Amount.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                var pm = dto.PaymentMethod.Trim().ToLower();
                if (!AllowedPaymentMethods.Contains(pm))
                    throw new ArgumentException($"Invalid payment method '{dto.PaymentMethod}'. Allowed: {string.Join(", ", AllowedPaymentMethods)}.");
                expense.PaymentMethod = pm;
            }

            if (dto.ExpenseDate.HasValue)
                expense.ExpenseDate = dto.ExpenseDate.Value;

            if (dto.ReceiptUrl != null)
                expense.ReceiptUrl = dto.ReceiptUrl.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(expenseId, cancellationToken);
        }

        public async Task DeleteAsync(int expenseId, CancellationToken cancellationToken = default)
        {
            var expense = await _context.HotelExpenses
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId, cancellationToken)
                ?? throw new KeyNotFoundException($"Hotel expense record with ID {expenseId} was not found.");

            _context.HotelExpenses.Remove(expense);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static HotelExpenseResponseDto ToDto(HotelExpenseEntity e)
        {
            return new HotelExpenseResponseDto
            {
                ExpenseId = e.ExpenseId,
                HotelId = e.HotelId,
                HotelName = e.Hotel?.Name ?? string.Empty,
                ExpenseCategory = e.ExpenseCategory,
                Description = e.Description,
                Amount = e.Amount,
                PaymentMethod = e.PaymentMethod,
                ExpenseDate = e.ExpenseDate,
                ReceiptUrl = e.ReceiptUrl
            };
        }
    }
}
