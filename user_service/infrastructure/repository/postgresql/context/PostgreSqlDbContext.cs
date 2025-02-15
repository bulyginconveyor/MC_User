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
    
    public PostgreSqlDbContext()
    {
        _connectionString = "Server=localhost;Port=5432;Database=UserService;User Id=postgres;Password=postgres;";
        //Database.EnsureDeleted();
        Database.EnsureCreated();

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
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO: Придумать - где и когда брать строку подключения из .env файла
        optionsBuilder.UseNpgsql(_connectionString); 
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddConfigurations();
        
        base.OnModelCreating(modelBuilder);
    }
}
