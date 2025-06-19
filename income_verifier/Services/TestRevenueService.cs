using income_verifier.Repositories;
using income_verifier.Repositories.Interfaces;

namespace income_verifier.Services;

public class TestRevenueService(
    IContractRepository cr,
    IPaymentRepository pr
) : RevenueService(cr, pr, null!)
{
    protected override Task<decimal> GetExchangeRateAsync(string currencyCode)
    {
        return Task.FromResult(currencyCode.ToUpper() switch
        {
            "EUR" => 4.0m, 
            "USD" => 5.0m, 
            _ => 1.0m
        });
    }
}