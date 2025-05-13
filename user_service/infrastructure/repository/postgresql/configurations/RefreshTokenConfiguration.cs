using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using user_service.services.jwt_authentification;

namespace user_service.infrastructure.repository.postgresql.configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Token);
        builder.ToTable("refresh_tokens");
        
        builder.Property(x => x.Token).HasColumnName("token").IsRequired();
        builder.Property(x => x.AccessToken).HasColumnName("access_token").IsRequired();
        builder.Property(x => x.ExpiresAccessToken).HasColumnName("expires_access_token").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.Expires).HasColumnName("expires").IsRequired();
        
        builder.Property(x => x.Created).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.Revoked).HasColumnName("revoked");
    }
}
