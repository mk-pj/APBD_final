namespace income_verifier.Services.Interfaces;

public interface IRevenueService
{
    public Task<decimal> GetCurrentRevenueAsync(int? productId = null);
    public Task<decimal> GetExpectedRevenueAsync(int? productId = null);
    public Task<decimal> GetCurrentRevenueInCurrencyAsync(string currencyCode, int? productId = null);
    public Task<decimal> GetExpectedRevenueInCurrencyAsync(string currencyCode, int? productId = null);
}