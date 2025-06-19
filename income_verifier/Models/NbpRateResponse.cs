namespace income_verifier.Models;

public class NbpRateResponse
{
    public string Table { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string Code { get; set; } = null!;
    public List<NbpRate> Rates { get; set; } = null!;
}

public class NbpRate
{
    public string No { get; set; } = null!;
    public string EffectiveDate { get; set; } = null!;
    public decimal Mid { get; set; }
}