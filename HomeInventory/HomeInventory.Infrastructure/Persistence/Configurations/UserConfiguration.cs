using HomeInventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");


            builder.Property(x => x.UserName)
                   .HasColumnName("user_name")
                   .IsRequired();

            builder.HasIndex(x => x.UserName)
                   .IsUnique();

            builder.Property(x => x.PasswordHash)
                   .HasColumnName("password_hash")
                   .IsRequired();

            builder.Property(x => x.UserRole)
                   .HasColumnName("user_role");

        }
    }
}
