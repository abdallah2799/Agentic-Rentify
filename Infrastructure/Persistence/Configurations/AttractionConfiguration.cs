// Infrastructure -> Persistence -> Configurations -> AttractionConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agentic_Rentify.Core.Entities;

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

        builder.OwnsOne(a => a.ReviewSummary, rs =>
        {
            rs.ToJson();
            rs.OwnsOne(r => r.Criteria);
        });

        // حقول الـ List<string> البسيطة
        // EF Core 8+ handles List<string> as JSON primitive collections automatically.
        // We do not need to force HasColumnType("nvarchar(max)") - EF does this.
        // We just need to ensure the DB knows it's standard.
        // But to be safe and explicit, let's leave them or remove the configurations if defaults work.
        // actually HasColumnType is fine, but removing OwnsMany for Categories is key.

    }
}