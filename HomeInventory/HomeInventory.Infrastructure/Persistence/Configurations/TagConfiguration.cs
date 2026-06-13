using HomeInventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("tags");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(30);
            builder.Property(x => x.Color).HasColumnName("color").IsRequired().HasMaxLength(20);
            builder.Property(x => x.HouseholdId).HasColumnName("household_id").IsRequired();

            builder.HasIndex(x => x.HouseholdId);

            builder.HasMany(t => t.Items)
                   .WithMany(i => i.Tags)
                   .UsingEntity("item_tags",
                       l => l.HasOne(typeof(Item)).WithMany().HasForeignKey("item_id").OnDelete(DeleteBehavior.Cascade),
                       r => r.HasOne(typeof(Tag)).WithMany().HasForeignKey("tag_id").OnDelete(DeleteBehavior.Cascade));
        }
    }
}
