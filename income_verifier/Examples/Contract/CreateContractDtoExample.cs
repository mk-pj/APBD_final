using income_verifier.DTOs.Contract;
using Swashbuckle.AspNetCore.Filters;

namespace income_verifier.Examples.Contract;

public class CreateContractDtoExample : IExamplesProvider<CreateContractDto>
{
    public CreateContractDto GetExamples()
    {
        return new CreateContractDto
        {
            ClientId = 1,
            SoftwareId = 2,
            SoftwareVersion = "v2.1.0",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(5),
            SupportYears = 2
        };
    }
}