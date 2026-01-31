using HomeInventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("locations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");


            builder.Property<string>("_name")
                   .HasColumnName("name")
                   .IsRequired();

            builder.Property(x => x.ParentLocationId)
                   .HasColumnName("parent_id");

            builder.HasOne<Location>()
                   .WithMany()
                   .HasForeignKey(x => x.ParentLocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.LocationType)
                   .HasColumnName("location_type")
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasColumnName("description");

            builder.HasIndex(x => x.ParentLocationId)
                   .HasDatabaseName("ix_locations_parent_id");

            builder.HasIndex(x => new { x.HouseholdId, x.ParentLocationId, x.NormalizedName }).IsUnique();

            builder.Property(x => x.SortOrder)
                   .HasColumnName("sort_order")
                   .HasDefaultValue(0);

            builder.Property(x => x.HouseholdId)
                   .HasColumnName("househod_id")
                   .IsRequired();

        }
    }
}
