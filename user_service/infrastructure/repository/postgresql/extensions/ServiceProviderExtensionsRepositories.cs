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
        var server = Environment.GetEnvironmentVariable("DB_SERVER");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

        var connectionString = $"Server={server};Port={port};Database={database};User Id={user};Password={password}";

        services.AddScoped<DbContext, PostgreSqlDbContext>(s => new PostgreSqlDbContext(connectionString));
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDbRepository<User>, UserRepository>();
        services.AddScoped<IDbRepository<Role>, BaseRepository<Role>>();
    }
}
