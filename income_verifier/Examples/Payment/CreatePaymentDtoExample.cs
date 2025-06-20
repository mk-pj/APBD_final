using income_verifier.DTOs.Payment;
using Swashbuckle.AspNetCore.Filters;

namespace income_verifier.Examples.Payment;

public class CreatePaymentDtoExample : IExamplesProvider<CreatePaymentDto>
{
    public CreatePaymentDto GetExamples()
    {
        return new CreatePaymentDto
        {
            ContractId = 1,
            Amount = 2500.00m
        };
    }
}