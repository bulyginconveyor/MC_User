using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using user_service.domain.models;

namespace user_service.infrastructure.repository.postgresql.configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").IsRequired().HasDefaultValue(Guid.NewGuid());
        builder.ComplexProperty(x => x.UserName, xu =>
            xu.Property(s => s.Value).HasColumnName("username").IsRequired()
        );
        builder.ComplexProperty(x => x.Password, xp =>
                xp.Property(s => s.Value).HasColumnName("password").IsRequired()
        );
        builder.ComplexProperty(x => x.Email, xe =>
            xe.Property(s => s.Value).HasColumnName("email").IsRequired()
        );
        
        builder.Property(x => x.ConfirmEmail).HasColumnName("confirm_email");

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired().HasDefaultValue(DateTime.UtcNow);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasOne(x => x.Role).WithMany();
        builder.HasMany(x => x.RefreshTokens).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
