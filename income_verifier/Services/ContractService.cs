using income_verifier.DTOs.Contract;
using income_verifier.Middlewares;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using income_verifier.Services.Interfaces;

namespace income_verifier.Services;

public class ContractService(
    IContractRepository contractRepo,
    IClientRepository clientRepo,
    ISoftwareRepository softwareRepo,
    IDiscountRepository discountRepo
    ) : IContractService
{
    public async Task<int> CreateContractAsync(CreateContractDto dto)
    {
        if (dto.SupportYears < 0 || dto.SupportYears > 3)
            throw new ArgumentException("Support years must be between 0 and 3");
        
        var client = await clientRepo.GetByIdAsync(dto.ClientId)
            ?? throw new NotFoundException("Client does not exist");

        var software = await softwareRepo.GetByIdAsync(dto.SoftwareId)
            ?? throw new NotFoundException("Software does not exist");

        var activeContracts = 
            await contractRepo.GetActiveContractsForClientAndSoftwareAsync(dto.ClientId, dto.SoftwareId);
        if (activeContracts.Count != 0)
            throw new ConflictException("Client already has an active contract for this software");

        var days = (dto.EndDate - dto.StartDate).Days;
        if (days < 3 || days > 30)
            throw new ArgumentException("Contract period must be between 3 and 30 days");

        var discountPercent = 0m;
        var discounts = await discountRepo.GetActiveDiscountsAsync(DateTime.Today);
        if (discounts.Count != 0)
            discountPercent = discounts.Max(d => d.Percentage);

        var allContracts = await contractRepo.GetContractsByClientIdAsync(dto.ClientId);
        if (allContracts.Count != 0)
            discountPercent += 0.05m;

        var basePrice = software.Price;
        var supportCost = 1000m * dto.SupportYears;
        var total = (basePrice + supportCost) * (1 - discountPercent);

        var contract = new Contract
        {
            ClientId = dto.ClientId,
            SoftwareId = dto.SoftwareId,
            SoftwareVersion = dto.SoftwareVersion,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            SupportYears = dto.SupportYears,
            Price = total,
            IsSigned = false,
            IsDeleted = false,
            DiscountId = discounts.OrderByDescending(d => d.Percentage).FirstOrDefault()?.Id
        };

        await contractRepo.AddAsync(contract);

        return contract.Id;
    }
    
    public async Task<Contract> GetContractByIdAsync(int id)
        => await contractRepo.GetByIdAsync(id) ?? throw new NotFoundException("Contract not found");

    public async Task<List<Contract>> GetContractsByClientIdAsync(int clientId)
        => await contractRepo.GetContractsByClientIdAsync(clientId);

    public async Task DeleteContractAsync(int id)
    {
        var contract = await contractRepo.GetByIdAsync(id) 
                       ?? throw new NotFoundException("Contract not found");
        await contractRepo.DeleteAsync(id);
    }
}