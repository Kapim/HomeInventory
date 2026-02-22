using HomeInventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("items");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");


            builder.Property(x => x.Name)
                   .HasColumnName("name")
                   .IsRequired()
                   .HasField("_name");

            builder.Property(x => x.LocationId)
                   .HasColumnName("location_id")
                   .IsRequired();

            builder.Property(x => x.OwnerUserId)
                   .HasColumnName("owner_user_id")
                   .IsRequired();

            builder.HasIndex(x => x.OwnerUserId);

            builder.Property(x => x.PlacementNote)
                   .HasColumnName("placement_note");

            builder.Property(x => x.Description)
                   .HasColumnName("description");

            builder.Property(x => x.Quantity)
                   .HasColumnName("quantity");

            builder.HasIndex(x => x.LocationId);


        }

    }
}
