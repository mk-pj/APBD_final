using income_verifier.Models;

namespace income_verifier.Repositories.Interfaces;

public interface ISoftwareRepository
{
    Task<Software?> GetByIdAsync(int id);
    Task<List<Software>> GetAllAsync();
}