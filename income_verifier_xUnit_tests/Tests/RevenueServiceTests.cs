using income_verifier.Models;
using income_verifier.Repositories.Fake;
using income_verifier.Services;
using income_verifier.Services.Interfaces;

namespace income_verifier_xUnit_tests.Tests;

public class RevenueServiceTests
{
    private readonly FakeContractRepository _contractRepo;
    private readonly FakePaymentRepository _paymentRepo;
    private readonly IRevenueService _service;

    public RevenueServiceTests()
    {
        _contractRepo = new FakeContractRepository();
        _paymentRepo = new FakePaymentRepository();
        _service = new TestRevenueService(_contractRepo, _paymentRepo);
    }

    [Fact]
    public async Task GetCurrentRevenueAsync_ReturnsSumOfSignedContractPayments()
    {
        // Arrange
        _contractRepo.SeedContracts([
            new Contract { Id = 1, SoftwareId = 1, IsSigned = true, Price = 1000, EndDate = DateTime.Today.AddDays(1) },
            new Contract { Id = 2, SoftwareId = 1, IsSigned = false, Price = 500, EndDate = DateTime.Today.AddDays(1) }
        ]);
        _paymentRepo.SeedPayments([
            new Payment { Id = 1, ContractId = 1, Amount = 600, PaymentDate = DateTime.Today },
            new Payment { Id = 2, ContractId = 2, Amount = 400, PaymentDate = DateTime.Today }
        ]);

        // Act
        var result = await _service.GetCurrentRevenueAsync();

        // Assert
        Assert.Equal(600, result);
    }

    [Fact]
    public async Task GetExpectedRevenueAsync_ReturnsSumOfSignedAndActiveContracts()
    {
        // Arrange
        _contractRepo.SeedContracts([
            new Contract { Id = 1, SoftwareId = 1, IsSigned = true, Price = 1000, EndDate = DateTime.Today.AddDays(-2) },
            new Contract { Id = 2, SoftwareId = 1, IsSigned = false, Price = 800, EndDate = DateTime.Today.AddDays(3) },
            new Contract { Id = 3, SoftwareId = 2, IsSigned = false, Price = 1500, EndDate = DateTime.Today.AddDays(-5) }
        ]);

        // Act
        var result = await _service.GetExpectedRevenueAsync();

        // Assert
        Assert.Equal(1000 + 800, result);
    }

    [Fact]
    public async Task GetCurrentRevenueInCurrencyAsync_ReturnsRevenueDividedByRate()
    {
        // Arrange
        _contractRepo.SeedContracts([
            new Contract { Id = 1, SoftwareId = 1, IsSigned = true, Price = 1000, EndDate = DateTime.Today.AddDays(2) }
        ]);
        _paymentRepo.SeedPayments([
            new Payment { Id = 1, ContractId = 1, Amount = 800, PaymentDate = DateTime.Today }
        ]);

        // Act
        var result = await _service.GetCurrentRevenueInCurrencyAsync("EUR");

        // Assert
        Assert.Equal(200, result); // 800 PLN / 4.0 = 200 EUR
    }

    [Fact]
    public async Task GetExpectedRevenueInCurrencyAsync_ReturnsExpectedRevenueDividedByRate()
    {
        // Arrange
        _contractRepo.SeedContracts([
            new Contract { Id = 1, SoftwareId = 1, IsSigned = true, Price = 600, EndDate = DateTime.Today.AddDays(1) },
            new Contract { Id = 2, SoftwareId = 1, IsSigned = false, Price = 800, EndDate = DateTime.Today.AddDays(1) }
        ]);

        // Act
        var result = await _service.GetExpectedRevenueInCurrencyAsync("USD");

        // Assert
        Assert.Equal(280, result); // (600+800) / 5.0 = 280
    }

    [Fact]
    public async Task GetCurrentRevenueAsync_ReturnsZero_WhenNoSignedContracts()
    {
        // Arrange
        _contractRepo.SeedContracts([
            new Contract { Id = 1, SoftwareId = 1, IsSigned = false, Price = 700, EndDate = DateTime.Today.AddDays(2) }
        ]);
        _paymentRepo.SeedPayments([
            new Payment { Id = 1, ContractId = 1, Amount = 700, PaymentDate = DateTime.Today }
        ]);

        // Act
        var result = await _service.GetCurrentRevenueAsync();

        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public async Task GetCurrentRevenueAsync_WithProductId_ReturnsRevenueForThatProduct()
    {
        // Arrange
        _contractRepo.SeedContracts([
            new Contract { Id = 1, SoftwareId = 1, IsSigned = true, Price = 1000, EndDate = DateTime.Today.AddDays(1) },
            new Contract { Id = 2, SoftwareId = 2, IsSigned = true, Price = 800, EndDate = DateTime.Today.AddDays(1) }
        ]);
        _paymentRepo.SeedPayments([
            new Payment { Id = 1, ContractId = 1, Amount = 700, PaymentDate = DateTime.Today },
            new Payment { Id = 2, ContractId = 2, Amount = 600, PaymentDate = DateTime.Today }
        ]);

        // Act
        var result = await _service.GetCurrentRevenueAsync(2);

        // Assert
        Assert.Equal(600, result);
    }
}
