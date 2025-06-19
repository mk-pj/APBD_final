using income_verifier.Data;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace income_verifier.Repositories;

public class DiscountRepository(AppDbContext context) : IDiscountRepository
{
    public async Task<List<Discount>> GetActiveDiscountsAsync(DateTime date)
    {
        return await context.Discounts
            .Where(d => d.StartDate <= date && d.EndDate >= date)
            .ToListAsync();
    }

    public async Task<Discount?> GetByIdAsync(int id)
    {
        return await context.Discounts.FirstOrDefaultAsync(d => d.Id == id);
    }
}