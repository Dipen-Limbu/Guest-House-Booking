using System.Collections.Generic;
using System.Threading.Tasks;
using Guest_House.DTOs.Billing;

namespace Guest_House.Services.Billing
{
    public interface IExpenseChargeService
    {
        Task<IEnumerable<ExpenseChargeResponseDto>> GetAllAsync(int? stayId = null);
        Task<ExpenseChargeResponseDto?> GetByIdAsync(int chargeId);
        Task<BillingResult<ExpenseChargeResponseDto>> CreateAsync(ExpenseChargeCreateDto dto);
        Task<BillingResult<ExpenseChargeResponseDto>> UpdateAsync(int chargeId, ExpenseChargeUpdateDto dto);
        Task<BillingResult<bool>> DeleteAsync(int chargeId);
    }
}
