using income_verifier.DTOs;

namespace income_verifier.Services;

using Models;
using Repositories;
using Middlewares;

public class ClientService(IClientRepository clientRepo) : IClientService
{
    public async Task<int> AddIndividualClientAsync(IndividualClient client)
    {
        if (await clientRepo.PeselExistsAsync(client.Pesel))
            throw new ConflictException("Client with this PESEL already exists");

        await clientRepo.AddAsync(client);
        return client.Id;
    }

    public async Task<int> AddCompanyClientAsync(CompanyClient client)
    {
        if (await clientRepo.KrsExistsAsync(client.Krs))
            throw new ConflictException("Client with this KRS already exists");

        await clientRepo.AddAsync(client);
        return client.Id;
    }

    public async Task<Client> GetClientByIdAsync(int id)
    {
        var client = await clientRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("Client with this ID does not exist");
        return client;
    }

    public async Task<List<Client>> GetAllClientsAsync()
        => await clientRepo.GetAllAsync();

    public async Task UpdateClientAsync(Client client)
    {
        var existing = await clientRepo.GetByIdAsync(client.Id)
            ?? throw new NotFoundException("Client to update was not found");
        
        switch (client)
        {
            case IndividualClient ic:
            {
                if (existing is not IndividualClient existingClient)
                    throw new ArgumentException("Client type mismatch");
                if (!ic.Pesel.Equals(existingClient.Pesel))
                {
                    if(await clientRepo.PeselExistsAsync(ic.Pesel))
                        throw new ConflictException("Another client with this PESEL already exists");
                }
                break;
            }
            case CompanyClient cc:
            {
                if(existing is not CompanyClient existingClient)
                    throw new ArgumentException("Client type mismatch");
                if (!cc.Krs.Equals(existingClient.Krs))
                {
                    if (await clientRepo.KrsExistsAsync(cc.Krs))
                        throw new ConflictException("Another client with this KRS already exists");
                }
                break;
            }
        }
        
        await clientRepo.UpdateAsync(client);
    }
    
    public async Task UpdateIndividualClientAsync(int id, UpdateIndividualClientDto dto)
    {
        var client = await clientRepo.GetByIdAsync(id) as IndividualClient
            ?? throw new NotFoundException("Client to update was not found");

        if (dto.FirstName != null)
            client.FirstName = dto.FirstName;
        if (dto.LastName != null)
            client.LastName = dto.LastName;
        if (dto.Address != null)
            client.Address = dto.Address;
        if (dto.Email != null)
            client.Email = dto.Email;
        if (dto.Phone != null)
            client.Phone = dto.Phone;

        await clientRepo.UpdateAsync(client);
    }
    
    public async Task UpdateCompanyClientAsync(int id, UpdateCompanyClientDto dto)
    {
        var client = await clientRepo.GetByIdAsync(id) as CompanyClient
            ?? throw new NotFoundException("Client to update was not found");

        if (dto.CompanyName != null)
            client.CompanyName = dto.CompanyName;
        if (dto.Address != null)
            client.Address = dto.Address;
        if (dto.Email != null)
            client.Email = dto.Email;
        if (dto.Phone != null)
            client.Phone = dto.Phone;

        await clientRepo.UpdateAsync(client);
    }

    public async Task<int> DeleteIndividualClientAsync(int id)
    {
        var client = await clientRepo.GetByIdAsync(id);
        if (client is not IndividualClient ind)
            throw new NotFoundException("Client to delete does not exist");

        if (ind.IsDeleted)
            throw new ConflictException("Client with this ID is already deleted");

        await clientRepo.DeleteAsync(id);
        return id;
    }
}
