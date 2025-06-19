namespace income_verifier.DTOs.Contract;

public class CreateContractDto
{
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public string SoftwareVersion { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SupportYears { get; set; }
}