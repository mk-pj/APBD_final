namespace income_verifier.DTOs.Payment;

public class CreatePaymentDto
{
    public int ContractId { get; set; }
    public decimal Amount { get; set; }   
}
    