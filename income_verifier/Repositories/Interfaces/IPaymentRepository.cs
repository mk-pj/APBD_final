using income_verifier.Models;

namespace income_verifier.Repositories.Interfaces;

public interface IPaymentRepository
{
    public Task AddAsync(Payment payment);
    public Task<List<Payment>> GetByContractIdAsync(int contractId);
    public Task DeleteAllByContractIdAsync(int contractId);
    public Task BeginTransactionAsync();
    public Task CommitTransactionAsync();
    public Task RollbackTransactionAsync();
}