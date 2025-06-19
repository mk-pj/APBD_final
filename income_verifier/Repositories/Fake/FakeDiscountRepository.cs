using income_verifier.Models;
using income_verifier.Repositories.Interfaces;

namespace income_verifier.Repositories.Fake;

public class FakeDiscountRepository : IDiscountRepository
{
    private readonly List<Discount> _discounts =
    [
        new Discount
        {
            Id = 1, Name = "Spring Promo", Percentage = 0.10m,
            StartDate = DateTime.Today.AddDays(-30), EndDate = DateTime.Today.AddDays(30)
        },

        new Discount
        {
            Id = 2, Name = "Summer Sale", Percentage = 0.20m,
            StartDate = DateTime.Today.AddDays(60), EndDate = DateTime.Today.AddDays(90)
        }
    ];

    public Task<List<Discount>> GetActiveDiscountsAsync(DateTime date)
        => Task.FromResult(_discounts.Where(d => d.StartDate <= date && d.EndDate >= date).ToList());

    public Task<Discount?> GetByIdAsync(int id)
        => Task.FromResult(_discounts.FirstOrDefault(d => d.Id == id));
}