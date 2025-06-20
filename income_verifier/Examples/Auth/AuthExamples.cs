using income_verifier.DTOs.AppUser;
using Swashbuckle.AspNetCore.Filters;

namespace income_verifier.Examples.Auth;

public class LoginRequestDtoExample : IExamplesProvider<LoginRequestDto>
{
    public LoginRequestDto GetExamples()
    {
        return new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        };
    }
}

public class RegisterRequestDtoExample : IExamplesProvider<RegisterRequestDto>
{
    public RegisterRequestDto GetExamples()
    {
        return new RegisterRequestDto
        {
            Username = "nowyuzytkownik",
            Password = "bezpieczneHaslo123",
            IsAdmin = false
        };
    }
}