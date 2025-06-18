using income_verifier.Models;

namespace income_verifier.Repositories;

public interface ISoftwareRepository
{
    Task<Software?> GetByIdAsync(int id);
    Task<List<Software>> GetAllAsync();
}