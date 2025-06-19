using System.Text.Json;
using income_verifier.Middlewares;
using income_verifier.Models;
using income_verifier.Repositories;
using income_verifier.Repositories.Interfaces;
using income_verifier.Services.Interfaces;

namespace income_verifier.Services;

public class RevenueService(
    IContractRepository contractRepository,
    IPaymentRepository paymentRepository,
    IHttpClientFactory httpClientFactory
) : IRevenueService
{
    public async Task<decimal> GetCurrentRevenueAsync(int? productId = null)
    {
        var contracts = await contractRepository.GetAllAsync();
        if (productId.HasValue)
            contracts = contracts.Where(c => c.SoftwareId == productId.Value).ToList();

        var signedContracts = contracts
            .Where(c => c.IsSigned)
            .Select(c => c.Id)
            .ToList();

        decimal sum = 0;
        foreach (var contractId in signedContracts)
        {
            var contractPayments = await paymentRepository.GetByContractIdAsync(contractId);
            sum += contractPayments.Sum(p => p.Amount);
        }

        return sum;
    }

    public async Task<decimal> GetExpectedRevenueAsync(int? productId = null)
    {
        var contracts = await contractRepository.GetAllAsync();
        if (productId.HasValue)
            contracts = contracts.Where(c => c.SoftwareId == productId.Value).ToList();

        var validContracts = contracts
            .Where(c => c.IsSigned || c.EndDate >= DateTime.Today)
            .ToList();

        return validContracts.Sum(c => c.Price);
    }

    public async Task<decimal> GetCurrentRevenueInCurrencyAsync(string currencyCode, int? productId = null)
    {
        var revenuePLN = await GetCurrentRevenueAsync(productId);
        var rate = await GetExchangeRateAsync(currencyCode);
        return Math.Round(revenuePLN / rate, 2);
    }

    public async Task<decimal> GetExpectedRevenueInCurrencyAsync(string currencyCode, int? productId = null)
    {
        var revenuePLN = await GetExpectedRevenueAsync(productId);
        var rate = await GetExchangeRateAsync(currencyCode);
        return Math.Round(revenuePLN / rate, 2);
    }

    protected virtual async Task<decimal> GetExchangeRateAsync(string currencyCode)
    {
        if (currencyCode.Equals("PLN", StringComparison.OrdinalIgnoreCase))
            return 1m;

        var httpClient = httpClientFactory.CreateClient();
        var url = $"https://api.nbp.pl/api/exchangerates/rates/a/{currencyCode.ToLower()}?format=json";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"Could not retrieve rate for {currencyCode} from NBP.");

        var content = await response.Content.ReadAsStringAsync();

        var nbpRate = 
            JsonSerializer.Deserialize<NbpRateResponse>(content, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        return nbpRate?.Rates.FirstOrDefault()?.Mid ?? throw new NotFoundException("No rate found!");
    }
}