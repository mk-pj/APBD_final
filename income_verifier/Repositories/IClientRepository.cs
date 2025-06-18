using income_verifier.Models;

namespace income_verifier.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task DeleteAsync(int id);
    Task<bool> PeselExistsAsync(string pesel);
    Task<bool> KrsExistsAsync(string krs);
    Task<List<Client>> GetAllAsync();
}