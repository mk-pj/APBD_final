using System.ComponentModel.DataAnnotations;

namespace income_verifier.Models;

public class Discount
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}