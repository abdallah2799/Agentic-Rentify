// Infrastructure -> Persistence -> Configurations -> TripConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agentic_Rentify.Core.Entities;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips");

        builder.Property(t => t.Title).IsRequired().HasMaxLength(250);
        builder.Property(t => t.City).HasMaxLength(150);
        builder.Property(t => t.Price).HasPrecision(18, 2);
        builder.Property(t => t.StartDate);

        // تخزين الـ Itinerary كـ JSON (Nested List)
        builder.OwnsMany(t => t.Itinerary, i =>
        {
            i.ToJson();
            i.OwnsMany(d => d.Activities); // الـ Activities جوه الـ Day
        });

        // تخزين معلومات الفندق المرتبط بالرحلة كـ JSON
        builder.OwnsOne(t => t.HotelInfo, h => { h.ToJson(); });

        builder.Property(t => t.AvailableDates).HasColumnType("nvarchar(max)");
        builder.Property(t => t.Images).HasColumnType("nvarchar(max)");
    }
}