using income_verifier.Data;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace income_verifier.Repositories;

public class SoftwareRepository(AppDbContext context) : ISoftwareRepository
{
    public async Task<Software?> GetByIdAsync(int id)
    {
        return await context.Software.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Software>> GetAllAsync()
    {
        return await context.Software.ToListAsync();
    }
}