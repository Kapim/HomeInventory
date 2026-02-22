using HomeInventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations
{
    public class HouseholdConfiguration : IEntityTypeConfiguration<Household>
    {
        public void Configure(EntityTypeBuilder<Household> builder)
        {
            builder.ToTable("households");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");


            builder.Property(x => x.Name)
                   .HasColumnName("name")
                   .IsRequired()
                   .HasField("_name");      

        }
    }
}
