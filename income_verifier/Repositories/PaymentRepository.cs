using income_verifier.Data;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace income_verifier.Repositories;

public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    public async Task AddAsync(Payment payment)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Payments.AddAsync(payment);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Payment>> GetByContractIdAsync(int contractId)
    {
        return await context.Payments
            .Where(p => p.ContractId == contractId)
            .ToListAsync();
    }

    public async Task DeleteAllByContractIdAsync(int contractId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var payments = await context.Payments
                .Where(p => p.ContractId == contractId)
                .ToListAsync();
            context.Payments.RemoveRange(payments);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
