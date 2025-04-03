using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using user_service.domain.models;

namespace user_service.infrastructure.repository.postgresql.configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").IsRequired().HasDefaultValue(Guid.NewGuid());
        builder.ComplexProperty(x => x.Name, xn =>
            xn.Property(s => s.Value).HasColumnName("name").IsRequired()
        );
        
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired().HasDefaultValue(DateTime.UtcNow);
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
    }
}
