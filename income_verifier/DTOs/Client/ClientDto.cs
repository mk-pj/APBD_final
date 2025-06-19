using System.ComponentModel.DataAnnotations;

namespace income_verifier.DTOs.Client;

public class ClientDto
{
    public int Id { get; set; }
    public string ClientType { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Address { get; set; } = null!;
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Phone]
    public string Phone { get; set; } = null!;
}
