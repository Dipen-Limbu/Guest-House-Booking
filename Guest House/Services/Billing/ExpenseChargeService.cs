using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Billing;
using Guest_House.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Guest_House.Services.Billing
{
    public class ExpenseChargeService : IExpenseChargeService
    {
        private readonly GuestHouseContext _context;
        private readonly ILogger<ExpenseChargeService> _logger;

        public ExpenseChargeService(
            GuestHouseContext context,
            ILogger<ExpenseChargeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ExpenseChargeResponseDto>> GetAllAsync(int? stayId = null)
        {
            var query = _context.ExpenseCharges
                .Include(c => c.RoomOrder)
                .AsNoTracking();

            if (stayId.HasValue && stayId.Value > 0)
            {
                query = query.Where(c => c.StayId == stayId.Value);
            }

            var charges = await query
                .OrderByDescending(c => c.IncurredAt)
                .ToListAsync();

            return charges.Select(MapToResponseDto);
        }

        public async Task<ExpenseChargeResponseDto?> GetByIdAsync(int chargeId)
        {
            if (chargeId <= 0)
            {
                return null;
            }

            var charge = await _context.ExpenseCharges
                .Include(c => c.RoomOrder)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ChargeId == chargeId);

            return charge == null ? null : MapToResponseDto(charge);
        }

        public async Task<BillingResult<ExpenseChargeResponseDto>> CreateAsync(
            ExpenseChargeCreateDto dto)
        {
            if (dto.StayId <= 0)
            {
                return BillingResult<ExpenseChargeResponseDto>.Failure(
                    "A valid positive StayId is required.",
                    StatusCodes.Status400BadRequest);
            }

            var errors = new List<string>();

            // 1. Validate Stay existence
            var stay = await _context.Stays.FindAsync(dto.StayId);

            if (stay == null)
            {
                errors.Add($"Stay with ID {dto.StayId} does not exist.");
            }

            // 2. Validate ChargeType
            if (!BillingConstants.IsValidChargeType(dto.ChargeType))
            {
                errors.Add(
                    $"Invalid charge_type '{dto.ChargeType}'. " +
                    $"Allowed values: {string.Join(", ", BillingConstants.AllowedChargeTypes)}.");
            }

            // 3. Validate Amount
            if (dto.Amount <= 0)
            {
                errors.Add("Amount must be a positive value greater than zero.");
            }

            // 4. Validate RoomOrderId relationship if provided
            if (dto.RoomOrderId.HasValue && dto.RoomOrderId.Value > 0)
            {
                var roomOrder = await _context.RoomOrders
                    .FindAsync(dto.RoomOrderId.Value);

                if (roomOrder == null)
                {
                    errors.Add(
                        $"Room order with ID {dto.RoomOrderId.Value} does not exist.");
                }
                else if (roomOrder.StayId != dto.StayId)
                {
                    errors.Add(
                        $"Room order with ID {dto.RoomOrderId.Value} " +
                        $"does not belong to Stay ID {dto.StayId}.");
                }
            }

            if (errors.Count > 0)
            {
                return BillingResult<ExpenseChargeResponseDto>.Failure(
                    "Expense charge validation failed.",
                    StatusCodes.Status400BadRequest,
                    errors);
            }

            var expenseCharge = new ExpenseCharge
            {
                StayId = dto.StayId,
                ChargeType = dto.ChargeType.Trim().ToLowerInvariant(),
                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),
                RoomOrderId =
                    (dto.RoomOrderId.HasValue && dto.RoomOrderId.Value > 0)
                        ? dto.RoomOrderId
                        : null,
                Amount = dto.Amount,
                IncurredAt = dto.IncurredAt ?? DateTime.UtcNow
            };

            _context.ExpenseCharges.Add(expenseCharge);

            await _context.SaveChangesAsync();

            // Re-query from database to guarantee persisted values
            // and load the RoomOrder navigation property.
            var persistedCharge = await _context.ExpenseCharges
                .Include(c => c.RoomOrder)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ChargeId == expenseCharge.ChargeId);

            _logger.LogInformation(
                "Created expense charge {ChargeId} for Stay {StayId} " +
                "of type {ChargeType} with amount {Amount}",
                expenseCharge.ChargeId,
                expenseCharge.StayId,
                expenseCharge.ChargeType,
                expenseCharge.Amount);

            return BillingResult<ExpenseChargeResponseDto>.Success(
                MapToResponseDto(persistedCharge ?? expenseCharge),
                "Expense charge created successfully.",
                StatusCodes.Status201Created);
        }

        public async Task<BillingResult<ExpenseChargeResponseDto>> UpdateAsync(
            int chargeId,
            ExpenseChargeUpdateDto dto)
        {
            if (chargeId <= 0)
            {
                return BillingResult<ExpenseChargeResponseDto>.Failure(
                    "A valid positive Charge ID is required.",
                    StatusCodes.Status400BadRequest);
            }

            var charge = await _context.ExpenseCharges
                .Include(c => c.RoomOrder)
                .FirstOrDefaultAsync(c => c.ChargeId == chargeId);

            if (charge == null)
            {
                return BillingResult<ExpenseChargeResponseDto>.Failure(
                    $"Expense charge with ID {chargeId} was not found.",
                    StatusCodes.Status404NotFound);
            }

            var errors = new List<string>();

            // 1. Validate ChargeType if provided
            if (!string.IsNullOrWhiteSpace(dto.ChargeType))
            {
                if (!BillingConstants.IsValidChargeType(dto.ChargeType))
                {
                    errors.Add(
                        $"Invalid charge_type '{dto.ChargeType}'. " +
                        $"Allowed values: {string.Join(", ", BillingConstants.AllowedChargeTypes)}.");
                }
                else
                {
                    charge.ChargeType = dto.ChargeType.Trim().ToLowerInvariant();
                }
            }

            // 2. Validate Amount if provided
            if (dto.Amount.HasValue)
            {
                if (dto.Amount.Value <= 0)
                {
                    errors.Add("Amount must be a positive value greater than zero.");
                }
                else
                {
                    charge.Amount = dto.Amount.Value;
                }
            }

            // 3. Validate RoomOrderId if provided
            //
            // If a RoomOrderId is supplied, make sure the room order exists
            // and belongs to the same stay.
            if (dto.RoomOrderId.HasValue)
            {
                if (dto.RoomOrderId.Value <= 0)
                {
                    errors.Add("RoomOrderId must be a positive value when provided.");
                }
                else
                {
                    var roomOrder = await _context.RoomOrders
                        .FindAsync(dto.RoomOrderId.Value);

                    if (roomOrder == null)
                    {
                        errors.Add(
                            $"Room order with ID {dto.RoomOrderId.Value} does not exist.");
                    }
                    else if (roomOrder.StayId != charge.StayId)
                    {
                        errors.Add(
                            $"Room order with ID {dto.RoomOrderId.Value} " +
                            $"does not belong to Stay ID {charge.StayId}.");
                    }
                    else
                    {
                        charge.RoomOrderId = dto.RoomOrderId.Value;
                    }
                }
            }

            if (errors.Count > 0)
            {
                return BillingResult<ExpenseChargeResponseDto>.Failure(
                    "Expense charge update validation failed.",
                    StatusCodes.Status400BadRequest,
                    errors);
            }

            // Update description only when supplied.
            if (dto.Description != null)
            {
                charge.Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim();
            }

            // Update incurred time only when supplied.
            if (dto.IncurredAt.HasValue)
            {
                charge.IncurredAt = dto.IncurredAt.Value;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Updated expense charge {ChargeId}",
                charge.ChargeId);

            // Re-query from database to guarantee persisted values
            // and load the RoomOrder navigation property.
            var persistedCharge = await _context.ExpenseCharges
                .Include(c => c.RoomOrder)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ChargeId == chargeId);

            return BillingResult<ExpenseChargeResponseDto>.Success(
                MapToResponseDto(persistedCharge ?? charge),
                "Expense charge updated successfully.");
        }

        public async Task<BillingResult<bool>> DeleteAsync(int chargeId)
        {
            if (chargeId <= 0)
            {
                return BillingResult<bool>.Failure(
                    "A valid positive Charge ID is required.",
                    StatusCodes.Status400BadRequest);
            }

            var charge = await _context.ExpenseCharges.FindAsync(chargeId);

            if (charge == null)
            {
                return BillingResult<bool>.Failure(
                    $"Expense charge with ID {chargeId} was not found.",
                    StatusCodes.Status404NotFound);
            }

            _context.ExpenseCharges.Remove(charge);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Deleted expense charge {ChargeId}",
                chargeId);

            return BillingResult<bool>.Success(
                true,
                "Expense charge deleted successfully.");
        }

        private static ExpenseChargeResponseDto MapToResponseDto(
            ExpenseCharge c)
        {
            return new ExpenseChargeResponseDto
            {
                ChargeId = c.ChargeId,
                StayId = c.StayId,
                ChargeType = c.ChargeType,
                Description = c.Description,
                RoomOrderId = c.RoomOrderId,
                RoomOrderNumber = c.RoomOrder?.OrderNumber,
                Amount = c.Amount,
                IncurredAt = c.IncurredAt
            };
        }
    }
}