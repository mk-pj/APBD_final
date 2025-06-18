using System.ComponentModel.DataAnnotations;

namespace income_verifier.Models;

public class CompanyClient : Client
{
    public string CompanyName { get; set; } = null!;
    
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid KRS length")]
    public string Krs { get; set; } = null!;
}
