// Infrastructure -> Persistence -> Configurations -> HotelConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agentic_Rentify.Core.Entities;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels");

        builder.Property(h => h.Name).IsRequired().HasMaxLength(200);
        builder.Property(h => h.BasePrice).HasPrecision(18, 2);

        // تخزين الغرف كـ JSON
        builder.OwnsMany(h => h.Rooms, r => 
        { 
            r.ToJson(); 
            r.Property(room => room.Price).HasPrecision(18, 2);
        });

        builder.Property(h => h.Facilities).HasColumnType("nvarchar(max)");
        builder.Property(h => h.Images).HasColumnType("nvarchar(max)");
    }
}