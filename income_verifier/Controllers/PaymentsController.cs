using income_verifier.DTOs.Payment;
using income_verifier.Examples.Payment;
using income_verifier.Mappers;
using income_verifier.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace income_verifier.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IPaymentService service) : ControllerBase
{

    [HttpGet("by-contract/{contractId:int}")]
    public async Task<ActionResult<List<PaymentDto>>> GetByContract(int contractId)
    {
        var payments = await service.GetPaymentsByContractIdAsync(contractId);
        var dtoList = payments.Select(PaymentMapper.ToDto).ToList();
        return Ok(dtoList);
    }

    [HttpGet("total-paid/{contractId:int}")]
    public async Task<ActionResult<decimal>> GetTotalPaid(int contractId)
    {
        var total = await service.GetTotalPaidAsync(contractId);
        return Ok(total);
    }

    [HttpGet("is-fully-paid/{contractId:int}")]
    public async Task<ActionResult<bool>> IsFullyPaid(int contractId)
    {
        var isPaid = await service.IsContractFullyPaidAsync(contractId);
        return Ok(isPaid);
    }
    
    [HttpPost]
    [SwaggerRequestExample(typeof(CreatePaymentDto), typeof(CreatePaymentDtoExample))]
    public async Task<ActionResult<PaymentDto>> Add([FromBody] CreatePaymentDto dto)
    {
        var id = await service.AddPaymentAsync(dto);
        var payments = await service.GetPaymentsByContractIdAsync(dto.ContractId);
        var payment = payments.FirstOrDefault(p => p.Id == id) 
                      ?? throw new Exception("Something went wrong during payment");
        return CreatedAtAction(nameof(GetByContract), new { contractId = dto.ContractId }, PaymentMapper.ToDto(payment));
    }
}