namespace income_verifier.Models;

public class Contract
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public int SoftwareId { get; set; }
    public Software Software { get; set; } = null!;

    public string SoftwareVersion { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public int SupportYears { get; set; }
    public bool IsSigned { get; set; }
    public bool IsDeleted { get; set; }

    public int? DiscountId { get; set; }
    public Discount Discount { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = null!;
}
