using income_verifier.Data;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace income_verifier.Repositories;

public class ClientRepository(AppDbContext context) : IClientRepository
{
    public async Task<Client?> GetByIdAsync(int id)
    {
        return await context.Clients.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Client client)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Clients.AddAsync(client);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateAsync(Client client)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            context.Clients.Update(client);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var client = await context.Clients.FindAsync(id);
            if (client is IndividualClient ind)
            {
                ind.IsDeleted = true;
                context.Clients.Update(ind);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> PeselExistsAsync(string pesel)
        => await context.Clients
            .OfType<IndividualClient>()
            .AnyAsync(c => c.Pesel == pesel);

    public async Task<bool> KrsExistsAsync(string krs)
        => await context.Clients
            .OfType<CompanyClient>()
            .AnyAsync(c => c.Krs == krs);

    public async Task<List<Client>> GetAllAsync()
    {
        var individuals = await context.Clients
            .OfType<IndividualClient>()
            .Where(ic => !ic.IsDeleted)
            .ToListAsync<Client>();
        
        var companies = await context.Clients
            .OfType<CompanyClient>()
            .ToListAsync<Client>();

        individuals.AddRange(companies);
        
        return individuals;
    }
}