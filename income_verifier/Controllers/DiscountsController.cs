using income_verifier.DTOs.Discount;
using income_verifier.Mappers;
using income_verifier.Middlewares;
using income_verifier.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace income_verifier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController(IDiscountRepository repository) : ControllerBase
{
    [HttpGet("active")]
    public async Task<ActionResult<List<DiscountDto>>> GetAll()
    {
        var list = await repository.GetActiveDiscountsAsync(DateTime.Today);
        var dtoList = list.Select(DiscountMapper.ToDto).ToList();
        return Ok(dtoList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DiscountDto>> GetById(int id)
    {
        var discount = await repository.GetByIdAsync(id) 
                       ?? throw new NotFoundException("Discount not found");
        return Ok(DiscountMapper.ToDto(discount));
    }
}