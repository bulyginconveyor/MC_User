using Microsoft.EntityFrameworkCore;
using user_service.domain.models;
using user_service.domain.models.valueobjects;
using user_service.infrastructure.repository.postgresql.extensions;

namespace user_service.infrastructure.repository.postgresql.context;

public class PostgreSqlDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    
    private readonly string _connectionString;
    
    public PostgreSqlDbContext(string connectionString)
    {
        _connectionString = connectionString;
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
        
        Database.Migrate();

        if (!Roles.Any())
        {
            Roles.AddRange(
                new Role(Name.Create("administrator")),
                new Role(Name.Create("moderator")),
                new Role(Name.Create("user"))
            );
            SaveChanges();
        }
    }
    public PostgreSqlDbContext(){}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString); 
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddConfigurations();
        
        base.OnModelCreating(modelBuilder);
    }
}
