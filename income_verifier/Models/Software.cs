namespace income_verifier.Models;

public class Software
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CurrentVersion { get; set; } = null!;
    public string Category { get; set; } = null!;
    
    public decimal Price { get; set; }

    public ICollection<Contract> Contracts { get; set; } = null!;
}