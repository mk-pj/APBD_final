using income_verifier.Data;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace income_verifier.Repositories;

public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    
    private IDbContextTransaction? _transaction;
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            try
            {
                await _transaction.CommitAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    public async Task AddAsync(Payment payment)
    {
        await context.Payments.AddAsync(payment);
        await context.SaveChangesAsync();
    }

    public async Task<List<Payment>> GetByContractIdAsync(int contractId)
    {
        return await context.Payments
            .Where(p => p.ContractId == contractId)
            .ToListAsync();
    }

    public async Task DeleteAllByContractIdAsync(int contractId)
    {
        var payments = await context.Payments
            .Where(p => p.ContractId == contractId)
            .ToListAsync();
        context.Payments.RemoveRange(payments);
        await context.SaveChangesAsync();
    }
}
