using income_verifier.DTOs.Payment;
using income_verifier.Middlewares;
using income_verifier.Models;
using income_verifier.Repositories.Interfaces;
using income_verifier.Services.Interfaces;

namespace income_verifier.Services;

public class PaymentService(
    IPaymentRepository paymentRepository, 
    IContractRepository contractRepository
) : IPaymentService
{
    public async Task<int> AddPaymentAsync(CreatePaymentDto dto)
    {
        var contract = await contractRepository.GetByIdAsync(dto.ContractId)
            ?? throw new NotFoundException("Contract not found");

        var totalPaid = await GetTotalPaidAsync(contract.Id);

        if (DateTime.Today > contract.EndDate)
        {
            if (totalPaid < contract.Price)
                await paymentRepository.DeleteAllByContractIdAsync(contract.Id);
            throw new ConflictException("Cannot pay after contract end date.");
        }

        if (totalPaid + dto.Amount > contract.Price)
            throw new ConflictException("Payment would exceed contract price.");

        var payment = new Payment
        {
            ContractId = contract.Id,
            Amount = dto.Amount,
            PaymentDate = DateTime.Today
        };
        
        await paymentRepository.AddAsync(payment);

        totalPaid += dto.Amount;

        if (!contract.IsSigned && totalPaid == contract.Price)
            await contractRepository.MarkAsSignedAsync(contract.Id);

        return payment.Id;
    }

    public async Task<List<Payment>> GetPaymentsByContractIdAsync(int contractId)
        => await paymentRepository.GetByContractIdAsync(contractId);

    public async Task<decimal> GetTotalPaidAsync(int contractId)
    {
        var payments = await paymentRepository.GetByContractIdAsync(contractId);
        return payments.Sum(p => p.Amount);
    }

    public async Task<bool> IsContractFullyPaidAsync(int contractId)
    {
        var contract = await contractRepository.GetByIdAsync(contractId)
            ?? throw new NotFoundException("Contract not found");

        var totalPaid = await GetTotalPaidAsync(contract.Id);
        return totalPaid == contract.Price;
    }
}
