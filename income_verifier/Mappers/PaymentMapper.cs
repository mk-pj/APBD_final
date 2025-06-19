using income_verifier.DTOs.Payment;
using income_verifier.Models;

namespace income_verifier.Mappers;

public static class PaymentMapper
{
    public static PaymentDto ToDto(Payment p) => new()
    {
        Id = p.Id,
        ContractId = p.ContractId,
        Amount = p.Amount,
        PaymentDate = p.PaymentDate
    };
}
