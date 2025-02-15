using Microsoft.EntityFrameworkCore;
using user_service.domain.models;
using user_service.infrastructure.repository.interfaces;
using user_service.infrastructure.repository.postgresql.context;
using user_service.infrastructure.repository.postgresql.repositories;
using user_service.infrastructure.repository.postgresql.repositories.@base;

namespace user_service.infrastructure.repository.postgresql.extensions;

public static class ServiceProviderExtensionsRepositories
{
    public static void AddPostgreSqlDbContext(this IServiceCollection services)
    {
        //TODO: Вносить строку подключения к БД
        services.AddScoped<DbContext, PostgreSqlDbContext>();
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDbRepository<User>, UserRepository>();
        services.AddScoped<IDbRepository<Role>, BaseRepository<Role>>();
    }
}
