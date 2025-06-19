using income_verifier.Models;

namespace income_verifier.Repositories.Interfaces;

public interface IDiscountRepository
{
    Task<List<Discount>> GetActiveDiscountsAsync(DateTime date);
    Task<Discount?> GetByIdAsync(int id);
}