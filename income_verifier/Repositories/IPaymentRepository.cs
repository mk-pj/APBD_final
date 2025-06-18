using income_verifier.Models;

namespace income_verifier.Repositories;

public interface IPaymentRepository
{
    public Task AddAsync(Payment payment);
    public Task<List<Payment>> GetByContractIdAsync(int contractId);
    public Task DeleteAllByContractIdAsync(int contractId);
}