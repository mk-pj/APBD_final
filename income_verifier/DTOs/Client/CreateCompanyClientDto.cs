namespace income_verifier.DTOs.Client;

public class CreateCompanyClientDto
{
    public string CompanyName { get; set; } = null!;
    public string Krs { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}