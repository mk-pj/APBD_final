namespace income_verifier.DTOs.Client;


public class CreateIndividualClientDto
{
    public string FirstName { get; set; } = null!;     
    public string LastName { get; set; } = null!;
    public string Pesel { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}