namespace income_verifier.DTOs.Discount;

public class DiscountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Percentage { get; set; }
}
