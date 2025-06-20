using income_verifier.DTOs.Discount;
using income_verifier.Models;

namespace income_verifier.Mappers;

public static class DiscountMapper
{
    public static DiscountDto ToDto(Discount discount)
        => new()
        {
            Id = discount.Id,
            Name = discount.Name,
            Percentage = discount.Percentage
        };
}
