using income_verifier.DTOs;
using income_verifier.DTOs.Contract;
using income_verifier.Middlewares;
using income_verifier.Repositories.Fake;
using income_verifier.Services;
using income_verifier.Services.Interfaces;

namespace income_verifier_xUnit_tests.Tests;

public class ContractServiceTests
{
    
    private readonly IContractService _contractService = new ContractService(
        new FakeContractRepository(),
        new FakeClientRepository(),
        new FakeSoftwareRepository(),
        new FakeDiscountRepository()
    );
    
    [Fact]
    public async Task CreateContractAsync_ShouldCalculateCorrectPrice_ForSuperAppAndDiscount()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ClientId = 2,
            SoftwareId = 1,
            SoftwareVersion = "1.0.0",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(10),
            SupportYears = 2
        };

        // Act
        var contractId = await _contractService.CreateContractAsync(dto);
        var contract = await _contractService.GetContractByIdAsync(contractId);

        // Assert
        var basePrice = 5000m;
        var supportCost = 1000m * 2;
        var discount = 0.10m;
        var expectedPrice = (basePrice + supportCost) * (1 - discount);

        Assert.Equal(expectedPrice, contract.Price);
        Assert.Equal(dto.ClientId, contract.ClientId);
        Assert.Equal(dto.SoftwareId, contract.SoftwareId);
        Assert.Equal(dto.SoftwareVersion, contract.SoftwareVersion);
        Assert.Equal(dto.SupportYears, contract.SupportYears);
    }
    
    
    [Fact]
    public async Task CreateContractAsync_ShouldIncludeReturningClientDiscount()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ClientId = 1,
            SoftwareId = 2,
            SoftwareVersion = "2.5.1",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(10),
            SupportYears = 1
        };

        // Act
        var contractId = await _contractService.CreateContractAsync(dto);
        var contract = await _contractService.GetContractByIdAsync(contractId);

        // Assert
        var basePrice = 12000m;
        var supportCost = 1000m * 1;
        var discount = 0.10m + 0.05m;
        var expectedPrice = (basePrice + supportCost) * (1 - discount);

        Assert.Equal(expectedPrice, contract.Price);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public async Task CreateContractAsync_ShouldThrow_WhenSupportYearsInvalid(int invalidYears)
    {
        var dto = new CreateContractDto
        {
            ClientId = 2,
            SoftwareId = 2,
            SoftwareVersion = "2.5.1",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(10),
            SupportYears = invalidYears
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _contractService.CreateContractAsync(dto));
    }
    
    [Fact]
    public async Task DeleteContractAsync_ShouldMarkContractAsDeleted()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            ClientId = 2,
            SoftwareId = 2,
            SoftwareVersion = "2.5.1",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(10),
            SupportYears = 1
        };

        var contractId = await _contractService.CreateContractAsync(dto);

        // Act
        await _contractService.DeleteContractAsync(contractId);

        // Assert
        // Próba pobrania usuniętego kontraktu powinna dać null/NotFoundException
        await Assert.ThrowsAsync<NotFoundException>(() => _contractService.GetContractByIdAsync(contractId));
    }
    
    
}