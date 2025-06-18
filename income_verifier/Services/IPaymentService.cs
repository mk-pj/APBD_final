using income_verifier.DTOs;
using income_verifier.Models;

namespace income_verifier.Services;

public interface IPaymentService
{
    Task<int> AddPaymentAsync(CreatePaymentDto dto);
    Task<List<Payment>> GetPaymentsByContractIdAsync(int contractId);
    Task<decimal> GetTotalPaidAsync(int contractId);
    Task<bool> IsContractFullyPaidAsync(int contractId);
}
