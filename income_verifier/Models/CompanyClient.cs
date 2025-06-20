using System.ComponentModel.DataAnnotations;

namespace income_verifier.Models;

public class CompanyClient : Client
{
    public string CompanyName { get; set; } = null!;
    public string Krs { get; set; } = null!;
}
