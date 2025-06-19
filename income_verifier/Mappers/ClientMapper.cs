using income_verifier.DTOs.Client;
using income_verifier.Models;

namespace income_verifier.Mappers;

public static class ClientMapper
{
    public static ClientDto ToDto(Client client)
    {
        return client switch
        {
            IndividualClient ic => new ClientDto
            {
                Id = ic.Id,
                ClientType = "Individual",
                DisplayName = $"{ic.FirstName} {ic.LastName}",
                Address = ic.Address,
                Email = ic.Email,
                Phone = ic.Phone
            },
            CompanyClient cc => new ClientDto
            {
                Id = cc.Id,
                ClientType = "Company",
                DisplayName = cc.CompanyName,
                Address = cc.Address,
                Email = cc.Email,
                Phone = cc.Phone
            },
            _ => throw new ArgumentException("Invalid client")
        };
    }

    public static IndividualClient FromCreateDto(CreateIndividualClientDto dto)
        => new IndividualClient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Pesel = dto.Pesel,
            Address = dto.Address,
            Email = dto.Email,
            Phone = dto.Phone
        };

    public static CompanyClient FromCreateDto(CreateCompanyClientDto dto)
        => new CompanyClient
        {
            CompanyName = dto.CompanyName,
            Krs = dto.Krs,
            Address = dto.Address,
            Email = dto.Email,
            Phone = dto.Phone
        };
}
