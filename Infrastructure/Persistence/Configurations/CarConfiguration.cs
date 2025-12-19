// Infrastructure -> Persistence -> Configurations -> CarConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agentic_Rentify.Core.Entities;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Price).HasPrecision(18, 2);

        builder.Property(c => c.Features).HasColumnType("nvarchar(max)");
        builder.Property(c => c.Images).HasColumnType("nvarchar(max)");
        builder.Property(c => c.Amenities).HasColumnType("nvarchar(max)");
    }
}