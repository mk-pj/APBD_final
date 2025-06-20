using income_verifier.Models;
using Microsoft.EntityFrameworkCore;

namespace income_verifier.Data;

public class AppDbContext : DbContext
{
    
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Software> Software { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    
    protected AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>().HasDiscriminator<string>("ClientType")
            .HasValue<IndividualClient>("Individual")
            .HasValue<CompanyClient>("Company");

        modelBuilder.Entity<Client>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Address).IsRequired().HasMaxLength(50);
            e.Property(x => x.Email).IsRequired().HasMaxLength(50);
            e.Property(x => x.Phone).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<IndividualClient>(e =>
        {
            e.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Pesel).IsRequired().HasMaxLength(11);
            e.HasIndex(x => x.Pesel).IsUnique();
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<CompanyClient>(e =>
        {
            e.Property(x => x.CompanyName).IsRequired().HasMaxLength(150);
            e.Property(x => x.Krs).IsRequired().HasMaxLength(10);
            e.HasIndex(x => x.Krs).IsUnique();
        });

        modelBuilder.Entity<Software>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).IsRequired().HasMaxLength(200);
            e.Property(x => x.CurrentVersion).IsRequired().HasMaxLength(20);
            e.Property(x => x.Category).IsRequired().HasMaxLength(50);
            e.Property(x => x.Price).IsRequired().HasPrecision(18,2);
        });

        modelBuilder.Entity<Discount>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Percentage).IsRequired().HasPrecision(5, 2);
            e.Property(x => x.StartDate).IsRequired();
            e.Property(x => x.EndDate).IsRequired();
        });

        modelBuilder.Entity<Contract>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SoftwareVersion).IsRequired().HasMaxLength(20);
            e.Property(x => x.Price).IsRequired().HasPrecision(18, 2);
            e.Property(x => x.StartDate).IsRequired();
            e.Property(x => x.EndDate).IsRequired();
            e.Property(x => x.SupportYears).HasDefaultValue(0);
            e.Property(x => x.IsSigned).HasDefaultValue(false);
            e.Property(x => x.IsDeleted).HasDefaultValue(false);

            e.HasOne(x => x.Client)
                .WithMany(c => c.Contracts)
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Software)
                .WithMany(s => s.Contracts)
                .HasForeignKey(x => x.SoftwareId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Discount)
                .WithMany()
                .HasForeignKey(x => x.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).IsRequired().HasPrecision(18, 2);
            e.Property(x => x.PaymentDate).IsRequired();
            
            e.HasOne(x => x.Contract)
                .WithMany(c => c.Payments)
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username)
                  .IsRequired()
                  .HasMaxLength(50);
            entity.Property(u => u.Password)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(u => u.IsAdmin).IsRequired();
        });
        
        
        modelBuilder.Entity<IndividualClient>().HasData(
            new IndividualClient
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Pesel = "12345678901",
                Address = "Main Street 1",
                Email = "john.doe@email.com",
                Phone = "111222333",
                IsDeleted = false
            },
            new IndividualClient
            {
                Id = 2,
                FirstName = "Anna",
                LastName = "Smith",
                Pesel = "98765432109",
                Address = "Second Avenue 2",
                Email = "anna.smith@email.com",
                Phone = "444555666",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<CompanyClient>().HasData(
            new CompanyClient
            {
                Id = 3,
                CompanyName = "Acme Corp",
                Krs = "0000111122",
                Address = "Business Road 10",
                Email = "info@acmecorp.com",
                Phone = "555666777"
            },
            new CompanyClient
            {
                Id = 4,
                CompanyName = "Tech Solutions",
                Krs = "9999888877",
                Address = "Tech Park 5",
                Email = "contact@techsolutions.com",
                Phone = "888999000"
            }
        );

        modelBuilder.Entity<Software>().HasData(
            new Software
            {
                Id = 1,
                Name = "SuperApp",
                Description = "Sample software 1",
                CurrentVersion = "1.0.0",
                Category = "Business",
                Price = 5000m
            },
            new Software
            {
                Id = 2,
                Name = "MegaTool",
                Description = "Sample software 2",
                CurrentVersion = "2.5.1",
                Category = "Utility",
                Price = 12000m
            }
        );

        modelBuilder.Entity<Discount>().HasData(
            new Discount
            {
                Id = 1,
                Name = "Spring Promo",
                Percentage = 0.10m,
                StartDate = DateTime.Today.AddDays(-30),
                EndDate = DateTime.Today.AddDays(30)
            },
            new Discount
            {
                Id = 2,
                Name = "Summer Sale",
                Percentage = 0.20m,
                StartDate = DateTime.Today.AddDays(60),
                EndDate = DateTime.Today.AddDays(90)
            }
        );
        
        modelBuilder.Entity<Contract>().HasData(
            new Contract
            {
                Id = 1,
                ClientId = 1,
                SoftwareId = 1,
                SoftwareVersion = "1.0.0",
                Price = 5000m,
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today.AddYears(1),
                SupportYears = 1,
                IsSigned = true,
                IsDeleted = false,
                DiscountId = 1 
            },
            new Contract
            {
                Id = 2,
                ClientId = 3, 
                SoftwareId = 2,
                SoftwareVersion = "2.5.1",
                Price = 12000m,
                StartDate = DateTime.Today.AddDays(-20),
                EndDate = DateTime.Today.AddYears(2),
                SupportYears = 2,
                IsSigned = false,
                IsDeleted = false,
                DiscountId = null 
            }
        );
        
        modelBuilder.Entity<Payment>().HasData(
            new Payment
            {
                Id = 1,
                ContractId = 1,
                Amount = 5000m,
                PaymentDate = DateTime.Today.AddDays(-5)
            },
            new Payment
            {
                Id = 2,
                ContractId = 2,
                Amount = 6000m,
                PaymentDate = DateTime.Today.AddDays(-2)
            }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Password = "admin123", IsAdmin = true },
            new User { Id = 2, Username = "user", Password = "user123", IsAdmin = false }
        );
    }
}