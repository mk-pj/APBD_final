using income_verifier.DTOs.Software;
using income_verifier.Models;

namespace income_verifier.Mappers;

public static class SoftwareMapper
{
    public static SoftwareDto ToDto(Software software)
        => new()
        {
            Id = software.Id,
            Name = software.Name,
            Description = software.Description,
            CurrentVersion = software.CurrentVersion,
            Category = software.Category,
            Price = software.Price
        };
}
