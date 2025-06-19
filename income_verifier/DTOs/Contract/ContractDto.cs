namespace income_verifier.DTOs.Contract;

public class ContractDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public string SoftwareVersion { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public int SupportYears { get; set; }
    public bool IsSigned { get; set; }
}
