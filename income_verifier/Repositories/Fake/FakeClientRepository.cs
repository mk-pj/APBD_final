using income_verifier.Middlewares;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;

namespace income_verifier.Repositories.Fake;

public class FakeClientRepository : IClientRepository
{
    private readonly List<Client> _clients =
    [
        new IndividualClient
        {
            Id = 1,
            FirstName = "Jan",
            LastName = "Kowalski",
            Pesel = "12345678901",
            Address = "ul. Testowa 1",
            Email = "jan@kowalski.pl",
            Phone = "123456789",
            IsDeleted = false,
            Contracts = new List<Contract>()
        },

        new CompanyClient
        {
            Id = 2,
            CompanyName = "Firma Testowa",
            Krs = "0000123456",
            Address = "ul. Biznesowa 10",
            Email = "biuro@firma.pl",
            Phone = "987654321",
            Contracts = new List<Contract>()
        }
    ];

    public Task<Client?> GetByIdAsync(int id)
    {
        var client = _clients.FirstOrDefault(c => c.Id == id);
        
        if(client is IndividualClient ic && ic.IsDeleted == true)
            throw new NotFoundException("Individual client not found");
        
        return Task.FromResult(client);
    }

    public Task AddAsync(Client client)
    {
        if (client.Id == 0)
            client.Id = _clients.Count == 0 ? 1 : _clients.Max(c => c.Id) + 1;
        _clients.Add(client);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Client client)
    {
        var existing = _clients.FirstOrDefault(c => c.Id == client.Id);
        if (existing != null)
        {
            existing.Address = client.Address;
            existing.Email = client.Email;
            existing.Phone = client.Phone;

            if (existing is IndividualClient indExisting && client is IndividualClient indNew)
            {
                indExisting.FirstName = indNew.FirstName;
                indExisting.LastName = indNew.LastName;
                indExisting.Pesel = indNew.Pesel;
                indExisting.IsDeleted = indNew.IsDeleted;
            }
            else if (existing is CompanyClient compExisting && client is CompanyClient compNew)
            {
                compExisting.CompanyName = compNew.CompanyName;
                compExisting.Krs = compNew.Krs;
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var client = _clients.FirstOrDefault(c => c.Id == id);
        if (client is IndividualClient ind)
            ind.IsDeleted = true;
        return Task.CompletedTask;
    }

    public Task<bool> PeselExistsAsync(string pesel)
        => Task.FromResult(_clients.OfType<IndividualClient>().Any(c => c.Pesel == pesel));

    public Task<bool> KrsExistsAsync(string krs)
        => Task.FromResult(_clients.OfType<CompanyClient>().Any(c => c.Krs == krs));

    public Task<List<Client>> GetAllAsync()
    {
        var result = new List<Client>();
        
        foreach (var client in _clients)
        {
            if(client is IndividualClient ic && ic.IsDeleted == true)
                continue;
            result.Add(client);
        }
        
        return Task.FromResult(result);
    }
    
}