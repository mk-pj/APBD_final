using income_verifier.DTOs;
using income_verifier.DTOs.Contract;
using income_verifier.Models;

namespace income_verifier.Services.Interfaces;

public interface IContractService
{
    public Task<int> CreateContractAsync(CreateContractDto dto);
    public Task<Contract> GetContractByIdAsync(int id);
    public Task<List<Contract>> GetContractsByClientIdAsync(int clientId);
    public Task DeleteContractAsync(int id);
}