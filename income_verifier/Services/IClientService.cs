using income_verifier.DTOs;
using income_verifier.Models;

namespace income_verifier.Services;

public interface IClientService
{
    Task<int> AddIndividualClientAsync(IndividualClient client);
    Task<int> AddCompanyClientAsync(CompanyClient client);
    Task<Client> GetClientByIdAsync(int id);
    Task<List<Client>> GetAllClientsAsync();
    Task UpdateClientAsync(Client client);
    Task UpdateIndividualClientAsync(int id, UpdateIndividualClientDto dto);
    Task UpdateCompanyClientAsync(int id, UpdateCompanyClientDto dto);
    Task<int> DeleteIndividualClientAsync(int id);
}