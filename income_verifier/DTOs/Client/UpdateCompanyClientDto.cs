using System.ComponentModel.DataAnnotations;

namespace income_verifier.DTOs.Client;

public class UpdateCompanyClientDto
{
    public string? CompanyName { get; set; }
    public string? Address { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    [Phone]
    public string? Phone { get; set; }
}