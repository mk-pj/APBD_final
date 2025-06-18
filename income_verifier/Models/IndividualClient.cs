using System.ComponentModel.DataAnnotations;

namespace income_verifier.Models;

public class IndividualClient : Client
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid PESEL length")]
    public string Pesel { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
