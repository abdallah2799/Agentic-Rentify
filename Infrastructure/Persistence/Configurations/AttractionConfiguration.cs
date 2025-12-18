// Infrastructure -> Persistence -> Configurations -> AttractionConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AttractionConfiguration : IEntityTypeConfiguration<Attraction>
{
    public void Configure(EntityTypeBuilder<Attraction> builder)
    {
        builder.ToTable("Attractions");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Price).HasPrecision(18, 2);

        // تخزين المصفوفات والأشياء المعقدة كـ JSON
        builder.OwnsMany(a => a.Images, i => { i.ToJson(); });
        builder.OwnsMany(a => a.Categories, c => { c.ToJson(); });

        builder.OwnsOne(a => a.ReviewSummary, rs =>
        {
            rs.ToJson();
            rs.OwnsOne(r => r.Criteria);
        });

        // حقول الـ List<string> البسيطة
        builder.Property(a => a.Amenities).HasColumnType("nvarchar(max)");
        builder.Property(a => a.Highlights).HasColumnType("nvarchar(max)");
    }
}