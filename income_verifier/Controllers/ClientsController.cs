using income_verifier.DTOs.Client;
using income_verifier.Examples.Client;
using income_verifier.Mappers;
using income_verifier.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace income_verifier.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetAll()
    {
        var clients = await service.GetAllClientsAsync();
        var dtoList = clients.Select(ClientMapper.ToDto).ToList();
        return Ok(dtoList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        var client = await service.GetClientByIdAsync(id);
        var dto = ClientMapper.ToDto(client);
        return Ok(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("individual")]
    [SwaggerRequestExample(typeof(CreateIndividualClientDto), typeof(CreateIndividualClientDtoExample))]
    public async Task<ActionResult> CreateIndividual([FromBody] CreateIndividualClientDto dto)
    {
        var client = ClientMapper.FromCreateDto(dto);
        var id = await service.AddIndividualClientAsync(client);
        return CreatedAtAction(nameof(GetById), new { id }, dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("company")]
    [SwaggerRequestExample(typeof(CreateCompanyClientDto), typeof(CreateCompanyClientDtoExample))]
    public async Task<ActionResult> CreateCompany([FromBody] CreateCompanyClientDto dto)
    {
        var client = ClientMapper.FromCreateDto(dto);
        var id = await service.AddCompanyClientAsync(client);
        return CreatedAtAction(nameof(GetById), new { id }, dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("individual/{id:int}")]
    [SwaggerRequestExample(typeof(UpdateIndividualClientDto), typeof(UpdateIndividualClientDtoExample))]
    public async Task<IActionResult> UpdateIndividual(int id, [FromBody] UpdateIndividualClientDto dto)
    {
        await service.UpdateIndividualClientAsync(id, dto);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("company/{id:int}")]
    [SwaggerRequestExample(typeof(UpdateCompanyClientDto), typeof(UpdateCompanyClientDtoExample))]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyClientDto dto)
    {
        await service.UpdateCompanyClientAsync(id, dto);
        return NoContent();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteIndividualClientAsync(id);
        return NoContent();
    }
}