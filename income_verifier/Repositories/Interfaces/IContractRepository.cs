using income_verifier.Models;

namespace income_verifier.Repositories.Interfaces;

public interface IContractRepository
{
    public Task AddAsync(Contract contract);
    public Task<Contract?> GetByIdAsync(int id);
    public Task<List<Contract>> GetContractsByClientIdAsync(int clientId);
    public Task<List<Contract>> GetActiveContractsForClientAndSoftwareAsync(int clientId, int softwareId);
    public Task DeleteAsync(int id);
    public Task MarkAsSignedAsync(int contractId);
    public Task<List<Contract>> GetAllAsync();
}