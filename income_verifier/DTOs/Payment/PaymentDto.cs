namespace income_verifier.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}
