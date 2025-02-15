using Microsoft.EntityFrameworkCore;
using user_service.infrastructure.repository.postgresql.configurations;

namespace user_service.infrastructure.repository.postgresql.extensions;

public static class ModelBuilderExtensions
{
    public static void AddConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }
}
