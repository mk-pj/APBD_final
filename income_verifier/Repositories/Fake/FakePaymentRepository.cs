using income_verifier.Models;
using income_verifier.Repositories.Interfaces;

namespace income_verifier.Repositories.Fake;

public class FakePaymentRepository : IPaymentRepository
{
    private readonly List<Payment> _payments = [];

    public Task AddAsync(Payment payment)
    {
        payment.Id = _payments.Count > 0 ? _payments.Max(p => p.Id) + 1 : 1;
        _payments.Add(payment);
        return Task.CompletedTask;
    }

    public Task<List<Payment>> GetByContractIdAsync(int contractId)
    {
        var list = _payments.Where(p => p.ContractId == contractId).ToList();
        return Task.FromResult(list);
    }

    public Task DeleteAllByContractIdAsync(int contractId)
    {
        _payments.RemoveAll(p => p.ContractId == contractId);
        return Task.CompletedTask;
    }

    public Task BeginTransactionAsync() => Task.CompletedTask;

    public Task CommitTransactionAsync() => Task.CompletedTask;

    public Task RollbackTransactionAsync() => Task.CompletedTask;

    public void SeedPayments(IEnumerable<Payment> payments)
    {
        _payments.Clear();
        _payments.AddRange(payments);
    }

    public Task<Payment?> GetByIdAsync(int id)
        => Task.FromResult(_payments.FirstOrDefault(p => p.Id == id));
}