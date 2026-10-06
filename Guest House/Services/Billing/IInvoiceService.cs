using System.Collections.Generic;
using System.Threading.Tasks;
using Guest_House.DTOs.Billing;

namespace Guest_House.Services.Billing
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync(string? status = null);
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int invoiceId);
        Task<InvoiceResponseDto?> GetInvoiceByBookingIdAsync(int bookingId);
        Task<BillingResult<InvoiceResponseDto>> GenerateInvoiceAsync(GenerateInvoiceRequestDto request);
        Task<BillingResult<InvoiceResponseDto>> UpdateInvoiceAsync(int invoiceId, InvoiceUpdateDto dto);
        Task<IEnumerable<InvoiceItemResponseDto>> GetInvoiceItemsAsync(int? invoiceId = null);
        Task<InvoiceItemResponseDto?> GetInvoiceItemByIdAsync(int itemId);
        Task<BillingResult<InvoiceItemResponseDto>> AddInvoiceItemAsync(InvoiceItemCreateDto dto);
    }
}
