using income_verifier.DTOs;
using income_verifier.Middlewares;
using income_verifier.Models;
using income_verifier.Repositories;

namespace income_verifier.Services;

public class ContractService(
    IContractRepository contractRepo,
    IClientRepository clientRepo,
    ISoftwareRepository softwareRepo,
    IDiscountRepository discountRepo
    ) : IContractService
{
    
    private readonly IContractRepository _contractRepo = contractRepo;
    private readonly IClientRepository _clientRepo = clientRepo;
    private readonly ISoftwareRepository _softwareRepo = softwareRepo;
    private readonly IDiscountRepository _discountRepo = discountRepo;


    public async Task<int> CreateContractAsync(CreateContractDto dto)
    {
        if (dto.SupportYears < 0 || dto.SupportYears > 3)
            throw new ArgumentException("Support years must be between 0 and 3");
        
        // 1. Sprawdź czy klient istnieje
        var client = await _clientRepo.GetByIdAsync(dto.ClientId)
            ?? throw new NotFoundException("Client does not exist");

        // 2. Sprawdź czy software istnieje
        var software = await _softwareRepo.GetByIdAsync(dto.SoftwareId)
            ?? throw new NotFoundException("Software does not exist");

        // 3. Sprawdź czy klient nie ma już aktywnego kontraktu na ten soft
        var activeContracts = 
            await _contractRepo.GetActiveContractsForClientAndSoftwareAsync(dto.ClientId, dto.SoftwareId);
        if (activeContracts.Count != 0)
            throw new ConflictException("Client already has an active contract for this software");

        // 4. Sprawdź okres trwania umowy (3-30 dni)
        var days = (dto.EndDate - dto.StartDate).Days;
        if (days < 3 || days > 30)
            throw new ArgumentException("Contract period must be between 3 and 30 days");

        // 5. Wylicz najwyższą zniżkę
        var discountPercent = 0m;
        var discounts = await _discountRepo.GetActiveDiscountsAsync(DateTime.Today);
        if (discounts.Count != 0)
            discountPercent = discounts.Max(d => d.Percentage);

        // 6. Dodaj 5% rabatu dla powracającego klienta
        var allContracts = await _contractRepo.GetContractsByClientIdAsync(dto.ClientId);
        if (allContracts.Count != 0)
            discountPercent += 0.05m;

        // 7. Wylicz cenę (podstawa + lata wsparcia + rabat)
        var basePrice = software.Price;
        var supportCost = 1000m * dto.SupportYears;
        var total = (basePrice + supportCost) * (1 - discountPercent);

        // 8. Utwórz i zapisz kontrakt
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

        await _contractRepo.AddAsync(contract);

        return contract.Id;
    }
    
    public async Task<Contract> GetContractByIdAsync(int id)
        => await _contractRepo.GetByIdAsync(id) ?? throw new NotFoundException("Contract not found");

    public async Task<List<Contract>> GetContractsByClientIdAsync(int clientId)
        => await _contractRepo.GetContractsByClientIdAsync(clientId);

    public async Task DeleteContractAsync(int id)
    {
        var contract = await _contractRepo.GetByIdAsync(id) 
                       ?? throw new NotFoundException("Contract not found");
        await _contractRepo.DeleteAsync(id);
    }
}