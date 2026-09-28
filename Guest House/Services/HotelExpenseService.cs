using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Guest_House.Data;
using Guest_House.Models;
using Guest_House.DTOs.HotelExpense;
using System;

namespace Guest_House.Services
{
    public class HotelExpenseService
    {
        private readonly GuestHouseContext _context;

        public HotelExpenseService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelExpenseDto>> GetAllExpensesAsync()
        {
            return await _context.HotelExpenses
                .AsNoTracking()
                .Select(e => new HotelExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    HotelId = e.HotelId,
                    ExpenseCategory = e.ExpenseCategory,
                    Description = e.Description,
                    Amount = e.Amount,
                    PaymentMethod = e.PaymentMethod,
                    ExpenseDate = e.ExpenseDate,
                    ReceiptUrl = e.ReceiptUrl
                })
                .ToListAsync();
        }

        public async Task<HotelExpenseDto?> GetExpenseByIdAsync(int expenseId)
        {
            return await _context.HotelExpenses
                .AsNoTracking()
                .Where(e => e.ExpenseId == expenseId)
                .Select(e => new HotelExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    HotelId = e.HotelId,
                    ExpenseCategory = e.ExpenseCategory,
                    Description = e.Description,
                    Amount = e.Amount,
                    PaymentMethod = e.PaymentMethod,
                    ExpenseDate = e.ExpenseDate,
                    ReceiptUrl = e.ReceiptUrl
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<HotelExpenseDto>> GetExpensesByHotelIdAsync(int hotelId)
        {
            return await _context.HotelExpenses
                .AsNoTracking()
                .Where(e => e.HotelId == hotelId)
                .Select(e => new HotelExpenseDto
                {
                    ExpenseId = e.ExpenseId,
                    HotelId = e.HotelId,
                    ExpenseCategory = e.ExpenseCategory,
                    Description = e.Description,
                    Amount = e.Amount,
                    PaymentMethod = e.PaymentMethod,
                    ExpenseDate = e.ExpenseDate,
                    ReceiptUrl = e.ReceiptUrl
                })
                .ToListAsync();
        }

        public async Task<HotelExpenseDto?> CreateExpenseAsync(HotelExpenseCreateDto dto)
        {
            var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId);
            if (!hotelExists) return null;

            var expense = new HotelExpense
            {
                HotelId = dto.HotelId,
                ExpenseCategory = dto.ExpenseCategory,
                Description = dto.Description,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                ExpenseDate = dto.ExpenseDate ?? DateTime.Now,
                ReceiptUrl = dto.ReceiptUrl
            };

            _context.HotelExpenses.Add(expense);
            await _context.SaveChangesAsync();

            return new HotelExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                HotelId = expense.HotelId,
                ExpenseCategory = expense.ExpenseCategory,
                Description = expense.Description,
                Amount = expense.Amount,
                PaymentMethod = expense.PaymentMethod,
                ExpenseDate = expense.ExpenseDate,
                ReceiptUrl = expense.ReceiptUrl
            };
        }

        public async Task<HotelExpenseDto?> UpdateExpenseAsync(int expenseId, HotelExpenseUpdateDto dto)
        {
            var expense = await _context.HotelExpenses.FindAsync(expenseId);
            if (expense == null) return null;

            expense.ExpenseCategory = dto.ExpenseCategory;
            expense.Description = dto.Description;
            expense.Amount = dto.Amount;
            expense.PaymentMethod = dto.PaymentMethod;
            if (dto.ExpenseDate.HasValue)
            {
                expense.ExpenseDate = dto.ExpenseDate;
            }
            expense.ReceiptUrl = dto.ReceiptUrl;

            await _context.SaveChangesAsync();

            return new HotelExpenseDto
            {
                ExpenseId = expense.ExpenseId,
                HotelId = expense.HotelId,
                ExpenseCategory = expense.ExpenseCategory,
                Description = expense.Description,
                Amount = expense.Amount,
                PaymentMethod = expense.PaymentMethod,
                ExpenseDate = expense.ExpenseDate,
                ReceiptUrl = expense.ReceiptUrl
            };
        }

        public async Task<bool> DeleteExpenseAsync(int expenseId)
        {
            var expense = await _context.HotelExpenses.FindAsync(expenseId);
            if (expense == null) return false;

            _context.HotelExpenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int?> GetHotelIdForExpenseAsync(int expenseId)
        {
            var expense = await _context.HotelExpenses.AsNoTracking().FirstOrDefaultAsync(e => e.ExpenseId == expenseId);
            return expense?.HotelId;
        }
    }
}
