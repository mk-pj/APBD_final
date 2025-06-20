using income_verifier.DTOs.Software;
using income_verifier.Mappers;
using income_verifier.Middlewares;
using income_verifier.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace income_verifier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SoftwareController(ISoftwareRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SoftwareDto>>> GetAll()
    {
        var list = await repository.GetAllAsync();
        var dtoList = list.Select(SoftwareMapper.ToDto).ToList();
        return Ok(dtoList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SoftwareDto>> Get(int id)
    {
        var software = await repository.GetByIdAsync(id) 
                       ?? throw new NotFoundException("Software not found");
        return Ok(SoftwareMapper.ToDto(software));
    }
}
