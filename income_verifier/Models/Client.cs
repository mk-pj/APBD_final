using System.ComponentModel.DataAnnotations;

namespace income_verifier.Models;

public abstract class Client
{
    public int Id { get; set; }
    public string Address { get; set; } = null!;
    
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Phone]
    public string Phone { get; set; } = null!;

    public ICollection<Contract> Contracts { get; set; } = null!;
}
