using income_verifier.DTOs.Client;
using income_verifier.Models;
using income_verifier.Repositories.Fake;
using income_verifier.Services;
using income_verifier.Middlewares;
using income_verifier.Services.Interfaces;

namespace income_verifier_xUnit_tests.Tests;

public class ClientServiceTests
{

    private readonly IClientService _service;
    private readonly FakeClientRepository _clientRepo;

    public ClientServiceTests()
    {
        _clientRepo = new FakeClientRepository();
        _service = new ClientService(_clientRepo);
    }

    [Fact]
    public async Task AddIndividualClientAsync_ShouldAddClient_WhenPeselIsUnique()
    {
        // Arrange
        var client = new IndividualClient
        {
            FirstName = "Test",
            LastName = "User",
            Pesel = "11122233344",
            Address = "Any St.",
            Email = "test@example.com",
            Phone = "555666777"
        };

        // Act
        var id = await _service.AddIndividualClientAsync(client);

        // Assert
        Assert.Equal(3, id);
    }

    [Fact]
    public async Task AddIndividualClientAsync_ShouldThrowConflictException_WhenPeselExists()
    {
        // Arrange
        var client = new IndividualClient
        {
            FirstName = "Another",
            LastName = "User",
            Pesel = "12345678901",
            Address = "Any St.",
            Email = "test2@example.com",
            Phone = "222333444"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(() => _service.AddIndividualClientAsync(client));
        Assert.Equal("Client with this PESEL already exists", ex.Message);
    }

    [Fact]
    public async Task GetClientByIdAsync_ShouldReturnClient_WhenClientExists()
    {
        // Act
        var client = await _service.GetClientByIdAsync(1);

        // Assert
        Assert.NotNull(client);
        Assert.Equal(1, client.Id);
    }

    [Fact]
    public async Task GetClientByIdAsync_ShouldThrowNotFoundException_WhenClientDoesNotExist()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetClientByIdAsync(999));
        Assert.Equal("Client with this ID does not exist", ex.Message);
    }
    
    [Fact]
    public async Task GetAllClientsAsync_ShouldReturnAllClient_WhenClientsExist()
    {
        // Act & Assert
        var clients = await _service.GetAllClientsAsync();
        Assert.Equal(2, clients.Count);
    }
    

    [Fact]
    public async Task DeleteIndividualClientAsync_ShouldSoftDeleteClient_WhenClientExists()
    {
        // Arrange
        var client = new IndividualClient
        {
            FirstName = "Soft",
            LastName = "Delete",
            Pesel = "55566677788",
            Address = "Test St.",
            Email = "soft@delete.com",
            Phone = "777888999",
            IsDeleted = false,
        };
        var id = await _service.AddIndividualClientAsync(client);

        // Act
        var deletedId = await _service.DeleteIndividualClientAsync(id);

        // Assert
        Assert.Equal(id, deletedId);
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetClientByIdAsync(deletedId));
        Assert.Equal("Individual client not found", ex.Message);
    }

    [Fact]
    public async Task DeleteIndividualClientAsync_ShouldThrowNotFoundException_WhenClientNotFound()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteIndividualClientAsync(-1));
        Assert.Equal("Client to delete does not exist", ex.Message);
    }

    [Fact]
    public async Task UpdateClientAsync_ShouldThrowConflictException_WhenChangingPeselToExisting()
    {
        // Arrange
        var client = new IndividualClient
        {
            Id = 2,
            FirstName = "Test",
            LastName = "Conflict",
            Pesel = "12345678901",
            Address = "Somewhere",
            Email = "test@conflict.com",
            Phone = "999888777"
        };

        var newClient = new IndividualClient
        {
            FirstName = "Temp",
            LastName = "User",
            Pesel = "22233344455",
            Address = "Other",
            Email = "temp@user.com",
            Phone = "555999111"
        };
        var newId = await _service.AddIndividualClientAsync(newClient);

        client.Id = newId;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(() => _service.UpdateClientAsync(client));
        Assert.Equal("Another client with this PESEL already exists", ex.Message);
    }
    
    [Fact]
    public async Task UpdateClientAsync_ShouldUpdateIndividualClient_WhenDataIsCorrect()
    {
        // Arrange
        var client = new IndividualClient
        {
            FirstName = "Before",
            LastName = "Update",
            Pesel = "77788899900",
            Address = "Old Street",
            Email = "before@update.com",
            Phone = "123123123"
        };
        var id = await _service.AddIndividualClientAsync(client);

        var updatedClient = new IndividualClient
        {
            Id = id,
            FirstName = "After",
            LastName = "Update",
            Pesel = "77788899900",
            Address = "New Street",
            Email = "after@update.com",
            Phone = "456456456"
        };

        // Act
        await _service.UpdateClientAsync(updatedClient);

        // Assert
        var result = await _service.GetClientByIdAsync(id) as IndividualClient;
        Assert.NotNull(result);
        Assert.Equal("After", result.FirstName);
        Assert.Equal("New Street", result.Address);
        Assert.Equal("after@update.com", result.Email);
    }

    [Fact]
    public async Task UpdateClientAsync_ShouldUpdateCompanyClient_WhenDataIsCorrect()
    {
        // Arrange
        var company = new CompanyClient
        {
            CompanyName = "Old Company",
            Krs = "1234567890",
            Address = "Old Company St.",
            Email = "old@company.com",
            Phone = "222111333"
        };
        var id = await _service.AddCompanyClientAsync(company);

        var updatedCompany = new CompanyClient
        {
            Id = id,
            CompanyName = "New Company",
            Krs = "1234567890", // unchanged KRS
            Address = "New Company St.",
            Email = "new@company.com",
            Phone = "555444333"
        };

        // Act
        await _service.UpdateClientAsync(updatedCompany);

        // Assert
        var result = await _service.GetClientByIdAsync(id) as CompanyClient;
        Assert.NotNull(result);
        Assert.Equal("New Company", result.CompanyName);
        Assert.Equal("New Company St.", result.Address);
        Assert.Equal("new@company.com", result.Email);
    }
    
    
    [Fact]
    public async Task UpdateIndividualClientAsync_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var client = new IndividualClient
        {
            FirstName = "Adam",
            LastName = "Nowak",
            Pesel = "88899900011",
            Address = "Old Street",
            Email = "adam@nowak.com",
            Phone = "999888777"
        };
        var id = await _service.AddIndividualClientAsync(client);

        var patchDto = new UpdateIndividualClientDto
        {
            Email = "new@email.com"
        };

        // Act
        await _service.UpdateIndividualClientAsync(id, patchDto);

        // Assert
        var result = await _service.GetClientByIdAsync(id) as IndividualClient;
        Assert.NotNull(result);
        Assert.Equal("Adam", result.FirstName);
        Assert.Equal("new@email.com", result.Email);
        Assert.Equal("Nowak", result.LastName);
    }
    
    [Fact]
    public async Task UpdateCompanyClientAsync_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var company = new CompanyClient
        {
            CompanyName = "Old Company",
            Krs = "9876543210",
            Address = "Old Address",
            Email = "old@company.com",
            Phone = "111222333"
        };
        var id = await _service.AddCompanyClientAsync(company);

        var patchDto = new UpdateCompanyClientDto
        {
            CompanyName = "New Company",
        };

        // Act
        await _service.UpdateCompanyClientAsync(id, patchDto);

        // Assert
        var result = await _service.GetClientByIdAsync(id) as CompanyClient;
        Assert.NotNull(result);
        Assert.Equal("New Company", result.CompanyName);
        Assert.Equal("Old Address", result.Address);
        Assert.Equal("old@company.com", result.Email); 
        Assert.Equal("111222333", result.Phone); 
    }
}
