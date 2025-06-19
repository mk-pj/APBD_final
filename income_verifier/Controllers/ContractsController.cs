using income_verifier.DTOs.Contract;
using income_verifier.Mappers;
using income_verifier.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace income_verifier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController(IContractService contractService) : ControllerBase
{
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContractDto>> GetById(int id)
    {
        var contract = await contractService.GetContractByIdAsync(id);
        return Ok(ContractMapper.ToDto(contract));
    }
    
    [HttpPost]
    public async Task<ActionResult<ContractDto>> Create([FromBody] CreateContractDto dto)
    {
        var contractId = await contractService.CreateContractAsync(dto);
        var contract = await contractService.GetContractByIdAsync(contractId);
        return CreatedAtAction(nameof(GetById), new { id = contractId }, ContractMapper.ToDto(contract));
    }

    [HttpGet("/api/clients/{clientId:int}/contracts")]
    public async Task<ActionResult<List<ContractDto>>> GetByClient(int clientId)
    {
        var contracts = await contractService.GetContractsByClientIdAsync(clientId);
        var dtoList = contracts.Select(ContractMapper.ToDto).ToList();
        return Ok(dtoList);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await contractService.DeleteContractAsync(id);
        return NoContent();
    }
}