namespace income_verifier.Examples.Client;

using Swashbuckle.AspNetCore.Filters;
using income_verifier.DTOs.Client;


public class CreateIndividualClientDtoExample : IExamplesProvider<CreateIndividualClientDto>
{
    public CreateIndividualClientDto GetExamples()
    {
        return new CreateIndividualClientDto
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Pesel = "90010112345",
            Address = "ul. Warszawska 12, 00-001 Warszawa",
            Email = "jan.kowalski@example.com",
            Phone = "48500600700"
        };
    }
}

public class CreateCompanyClientDtoExample : IExamplesProvider<CreateCompanyClientDto>
{
    public CreateCompanyClientDto GetExamples()
    {
        return new CreateCompanyClientDto
        {
            CompanyName = "Acme Sp. z o.o.",
            Krs = "0000123456",
            Address = "ul. Przemysłowa 7, 60-100 Poznań",
            Email = "biuro@acme.pl",
            Phone = "48221234567"
        };
    }
}