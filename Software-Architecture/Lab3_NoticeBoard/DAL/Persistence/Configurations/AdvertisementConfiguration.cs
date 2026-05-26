using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DAL.Entities;

namespace DAL.Persistence.Configurations;

public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder.ToTable("Advertisements");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Content).IsRequired().HasMaxLength(2000);
        
        builder.Property(a => a.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasMany(a => a.Tags)
               .WithMany(t => t.Advertisements)
               .UsingEntity(j => j.ToTable("AdvertisementTags"));
    }
}