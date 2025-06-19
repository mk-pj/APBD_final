using income_verifier.Data;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace income_verifier.Repositories;

public class ContractRepository(AppDbContext context) : IContractRepository
{
    public async Task AddAsync(Contract contract)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Contracts.AddAsync(contract);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Contract?> GetByIdAsync(int id)
    {
        return await context.Contracts
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<List<Contract>> GetContractsByClientIdAsync(int clientId)
    {
        return await context.Contracts
            .Where(c => c.ClientId == clientId && !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<Contract>> GetActiveContractsForClientAndSoftwareAsync(int clientId, int softwareId)
    {
        return await context.Contracts
            .Where(c => c.ClientId == clientId && 
                    c.SoftwareId == softwareId && !c.IsDeleted && c.IsSigned)
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var contract = await context.Contracts.FirstOrDefaultAsync(c => c.Id == id);
            if (contract != null)
            {
                contract.IsDeleted = true;
                context.Contracts.Update(contract);
                await context.SaveChangesAsync();
            }
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task MarkAsSignedAsync(int contractId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var contract = await context.Contracts
                .FirstOrDefaultAsync(c => c.Id == contractId);
            if (contract != null)
            {
                contract.IsSigned = true;
                context.Contracts.Update(contract);
                await context.SaveChangesAsync();
            }
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Contract>> GetAllAsync()
    {
        return await context.Contracts.Where(c => !c.IsDeleted).ToListAsync();
    }
}