using income_verifier.Models;

namespace income_verifier.Repositories.Fake;

public class FakeContractRepository : IContractRepository
{
    
    private readonly List<Contract> _contracts =
    [
        new Contract
        {
            Id = 1, ClientId = 1, SoftwareId = 1, SoftwareVersion = "1.0.0", 
            Price = 5000, StartDate = DateTime.Today.AddDays(-100), 
            EndDate = DateTime.Today.AddDays(-90), SupportYears = 1,
            IsSigned = true, IsDeleted = false
        }
    ];

    public Task AddAsync(Contract contract)
    {
        if (contract.Id == 0)
            contract.Id = _contracts.Count == 0 ? 1 : _contracts.Max(c => c.Id) + 1;
        _contracts.Add(contract);
        return Task.CompletedTask;
    }

    public Task<Contract?> GetByIdAsync(int id)
        => Task.FromResult(_contracts.FirstOrDefault(c => c.Id == id && !c.IsDeleted));

    public Task<List<Contract>> GetContractsByClientIdAsync(int clientId)
        => Task.FromResult(_contracts.Where(c => c.ClientId == clientId && !c.IsDeleted).ToList());

    public Task<List<Contract>> GetActiveContractsForClientAndSoftwareAsync(int clientId, int softwareId)
        => Task.FromResult(_contracts
                    .Where(c => c.ClientId == clientId && 
                        c.SoftwareId == softwareId && !c.IsDeleted && c.IsSigned).ToList());

    public Task DeleteAsync(int id)
    {
        var contract = _contracts.FirstOrDefault(c => c.Id == id);
        if (contract != null)
            contract.IsDeleted = true;
        return Task.CompletedTask;
    }
    
    public Task MarkAsSignedAsync(int contractId)
    {
        var contract = _contracts.FirstOrDefault(c => c.Id == contractId);
        if (contract != null)
            contract.IsSigned = true;
        return Task.CompletedTask;
    }
    
    public void SeedContracts(IEnumerable<Contract> contracts)
    {
        _contracts.Clear();
        _contracts.AddRange(contracts);
    }
}
