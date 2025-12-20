// Infrastructure -> Persistence -> ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Attraction> Attractions { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<AgentExecutionLog> AgentExecutionLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. تطبيق كل الـ Configurations اللي في الـ Assembly ده أوتوماتيكياً
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // 2. إعادة تسمية جداول الـ Identity لتكون سهلة القراءة
        modelBuilder.Entity<ApplicationUser>(b => b.ToTable("Users"));
        modelBuilder.Entity<IdentityRole>(b => b.ToTable("Roles"));
        modelBuilder.Entity<IdentityUserRole<string>>(b => b.ToTable("UserRoles"));
        modelBuilder.Entity<IdentityUserClaim<string>>(b => b.ToTable("UserClaims"));
        modelBuilder.Entity<IdentityUserLogin<string>>(b => b.ToTable("UserLogins"));
        modelBuilder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("RoleClaims"));
        modelBuilder.Entity<IdentityUserToken<string>>(b => b.ToTable("UserTokens"));

    }

}



    

    
