using DotNetEnv;
using user_service.domain.logics;
using user_service.infrastructure.http_clients.notification_service;
using user_service.infrastructure.repository.postgresql.extensions;
using user_service.infrastructure.repository.redis;
using user_service.services.jwt_authentification;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Env.Load("dev.env");
}
// Add services to the container.
builder.Services.AddPostgreSqlDbContext();
builder.Services.AddRepositories();

builder.Services.AddRedisCache();
builder.Services.AddCacheStorages();
builder.Services.AddConfirmCodeStorage();

builder.Services.AddNotificationServiceKafka();

builder.Services.AddTransient<AuthLogic>();

builder.Services.AddJwtAuth();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
