using income_verifier.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace income_verifier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RevenuesController(IRevenueService service) : ControllerBase
{
    [HttpGet("current")]
    public async Task<ActionResult<decimal>> GetCurrent([FromQuery] int? productId = null)
    {
        var value = await service.GetCurrentRevenueAsync(productId);
        return Ok(value);
    }

    [HttpGet("expected")]
    public async Task<ActionResult<decimal>> GetExpected([FromQuery] int? productId = null)
    {
        var value = await service.GetExpectedRevenueAsync(productId);
        return Ok(value);
    }

    [HttpGet("current-in-currency")]
    public async Task<ActionResult<decimal>> GetCurrentInCurrency([FromQuery] string currencyCode, [FromQuery] int? productId = null)
    {
        var value = await service.GetCurrentRevenueInCurrencyAsync(currencyCode, productId);
        return Ok(value);
    }

    [HttpGet("expected-in-currency")]
    public async Task<ActionResult<decimal>> GetExpectedInCurrency([FromQuery] string currencyCode, [FromQuery] int? productId = null)
    {
        var value = await service.GetExpectedRevenueInCurrencyAsync(currencyCode, productId);
        return Ok(value);
    }
}