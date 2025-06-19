using income_verifier.Models;
using income_verifier.Repositories.Interfaces;

namespace income_verifier.Repositories.Fake;

public class FakeSoftwareRepository : ISoftwareRepository
{
    private readonly List<Software> _softwares =
    [
        new Software
        {
            Id = 1, Name = "SuperApp", Description = "Test",
            CurrentVersion = "1.0.0", Category = "Biznes", Price = 5000
        },

        new Software
        {
            Id = 2, Name = "MegaTool", Description = "Narzędzie",
            CurrentVersion = "2.5.1", Category = "Utility", Price = 12000
        }
    ];

    public Task<Software?> GetByIdAsync(int id)
        => Task.FromResult(_softwares.FirstOrDefault(s => s.Id == id));

    public Task<List<Software>> GetAllAsync()
        => Task.FromResult(_softwares.ToList());
}