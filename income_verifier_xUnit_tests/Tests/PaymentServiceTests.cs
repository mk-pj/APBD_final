using income_verifier.DTOs;
using income_verifier.Middlewares;
using income_verifier.Models;
using income_verifier.Repositories.Fake;
using income_verifier.Services;

namespace income_verifier_xUnit_tests.Tests;

public class PaymentServiceTests
{
    private readonly FakePaymentRepository _paymentRepo;
    private readonly FakeContractRepository _contractRepo;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _paymentRepo = new FakePaymentRepository();
        _contractRepo = new FakeContractRepository();
        _service = new PaymentService(_paymentRepo, _contractRepo);

        // Seed kontraktu o wartości 5000, kończy się dzisiaj + 1 dzień
        _contractRepo.SeedContracts([
            new Contract { Id = 1, ClientId = 1, SoftwareId = 1, Price = 5000, 
                IsSigned = false, StartDate = DateTime.Today.AddDays(-10), EndDate = DateTime.Today.AddDays(1) }
        ]);
    }

    [Fact]
    public async Task AddPaymentAsync_ShouldAddPayment_WhenUnderLimitAndWithinDate()
    {
        // Arrange
        var dto = new CreatePaymentDto { ContractId = 1, Amount = 2000 };

        // Act
        var paymentId = await _service.AddPaymentAsync(dto);

        // Assert
        var payments = await _service.GetPaymentsByContractIdAsync(1);
        Assert.Single(payments);
        Assert.Equal(2000, payments.First().Amount);
        Assert.Equal(paymentId, payments.First().Id);
    }

    [Fact]
    public async Task AddPaymentAsync_ShouldThrow_WhenPaymentExceedsLimit()
    {
        // Arrange
        await _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 1, Amount = 4000 });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 1, Amount = 2000 }));

        Assert.Equal("Payment would exceed contract price.", ex.Message);
    }

    [Fact]
    public async Task AddPaymentAsync_ShouldSignContract_WhenPaidFullAmount()
    {
        // Arrange
        await _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 1, Amount = 5000 });

        // Assert
        var contract = await _contractRepo.GetByIdAsync(1);
        Assert.True(contract.IsSigned);
    }

    [Fact]
    public async Task AddPaymentAsync_ShouldRefundAndThrow_WhenPayingAfterEndDate()
    {
        // Arrange: kontrakt kończy się dziś, więc można dodać płatność
        _contractRepo.SeedContracts(new[]
        {
            new Contract
            {
                Id = 2, ClientId = 1, SoftwareId = 1,
                Price = 5000, IsSigned = false,
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today // kontrakt jeszcze trwa
            }
        });

        // Dodaj wpłatę podczas trwania kontraktu
        await _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 2, Amount = 3000 });

        // Teraz "kończy się" kontrakt (symulujemy upływ czasu)
        var contract = await _contractRepo.GetByIdAsync(2);
        contract.EndDate = DateTime.Today.AddDays(-1); // kontrakt już nieaktualny

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 2, Amount = 2000 }));

        Assert.Equal("Cannot pay after contract end date. All previous payments have been refunded.", ex.Message);

        var payments = await _service.GetPaymentsByContractIdAsync(2);
        Assert.Empty(payments);
    }
    
    [Fact]
    public async Task GetTotalPaidAsync_ShouldReturnSumOfPayments()
    {
        // Arrange
        await _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 1, Amount = 1200 });
        await _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 1, Amount = 1300 });

        // Act
        var total = await _service.GetTotalPaidAsync(1);

        // Assert
        Assert.Equal(2500, total);
    }

    [Fact]
    public async Task IsContractFullyPaidAsync_ShouldReturnTrue_WhenPaid()
    {
        // Arrange
        await _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 1, Amount = 5000 });

        // Act
        var isPaid = await _service.IsContractFullyPaidAsync(1);

        // Assert
        Assert.True(isPaid);
    }

    [Fact]
    public async Task IsContractFullyPaidAsync_ShouldReturnFalse_WhenNotPaid()
    {
        // Arrange
        await _service.AddPaymentAsync(new CreatePaymentDto { ContractId = 1, Amount = 4999 });

        // Act
        var isPaid = await _service.IsContractFullyPaidAsync(1);

        // Assert
        Assert.False(isPaid);
    }
}
