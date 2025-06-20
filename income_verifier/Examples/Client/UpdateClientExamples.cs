using income_verifier.DTOs.Client;
using Swashbuckle.AspNetCore.Filters;

namespace income_verifier.Examples.Client;


public class UpdateIndividualClientDtoExample : IExamplesProvider<UpdateIndividualClientDto>
{
    public UpdateIndividualClientDto GetExamples()
    {
        return new UpdateIndividualClientDto
        {
            FirstName = "Janusz",
            LastName = "Nowak",
            Address = "ul. Nowa 5, 01-234 Kraków",
            Email = "janusz.nowak@nowyemail.com",
            Phone = "48555555555"
        };
    }
}

public class UpdateCompanyClientDtoExample : IExamplesProvider<UpdateCompanyClientDto>
{
    public UpdateCompanyClientDto GetExamples()
    {
        return new UpdateCompanyClientDto
        {
            CompanyName = "Acme S.A.",
            Address = "ul. Zmieniona 1, 00-950 Wrocław",
            Email = "kontakt@acmesa.pl",
            Phone = "48771234567"
        };
    }
}