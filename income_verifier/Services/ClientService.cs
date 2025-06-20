using income_verifier.DTOs.Client;
using income_verifier.Repositories.Interfaces;
using income_verifier.Services.Interfaces;

namespace income_verifier.Services;

using Models;
using Middlewares;

public class ClientService(IClientRepository clientRepo) : IClientService
{
    public async Task<int> AddIndividualClientAsync(IndividualClient client)
    {
        if(client.Pesel.Length != 11)
            throw new ArgumentException("PESEL must be 11 characters long");
        
        ValidateEmail(client.Email);
        ValidatePhone(client.Phone);
        
        if (await clientRepo.PeselExistsAsync(client.Pesel))
            throw new ConflictException("Client with this PESEL already exists");

        await clientRepo.AddAsync(client);
        return client.Id;
    }

    public async Task<int> AddCompanyClientAsync(CompanyClient client)
    {
        if(client.Krs.Length != 10)
            throw new ArgumentException("KRs must be 10 characters long");
        
        ValidateEmail(client.Email);
        ValidatePhone(client.Phone);
        
        if (await clientRepo.KrsExistsAsync(client.Krs))
            throw new ConflictException("Client with this KRS already exists");

        await clientRepo.AddAsync(client);
        return client.Id;
    }
    
    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Invalid email address");
    }

    private static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 9 || !phone.All(char.IsDigit))
            throw new ArgumentException("Invalid phone number");
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
        ValidateEmail(client.Email);
        ValidatePhone(client.Phone);
        
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
        {
           ValidateEmail(dto.Email); 
           client.Email = dto.Email;
        }

        if (dto.Phone != null)
        {
            ValidatePhone(dto.Phone);
            client.Phone = dto.Phone;
        }

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
        {
            ValidateEmail(dto.Email);
            client.Email = dto.Email;
        }

        if (dto.Phone != null)
        {
            ValidatePhone(dto.Phone);
            client.Phone = dto.Phone;
        }

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
