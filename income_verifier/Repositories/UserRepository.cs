using income_verifier.Data;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace income_verifier.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username) =>
        await context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}
