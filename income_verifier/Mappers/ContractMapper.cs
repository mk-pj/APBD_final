using income_verifier.DTOs.Contract;
using income_verifier.Models;

namespace income_verifier.Mappers;

public static class ContractMapper
{
    public static ContractDto ToDto(Contract contract)
    {
        return new ContractDto
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            SoftwareId = contract.SoftwareId,
            SoftwareVersion = contract.SoftwareVersion,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Price = contract.Price,
            SupportYears = contract.SupportYears,
            IsSigned = contract.IsSigned
        };
    }
}
