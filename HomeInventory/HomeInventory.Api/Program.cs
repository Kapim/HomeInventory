using HomeInventory.Application.Interfaces;
using HomeInventory.Application.UseCases;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Zadej JWT token"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();
builder.Services.AddScoped<ILocationUseCase, LocationUseCase>();
builder.Services.AddScoped<IHouseholdUseCase, HouseholdUseCase>();
builder.Services.AddScoped<IItemUseCase, ItemUseCase>();
builder.Services.AddScoped<ITagUseCase, TagUseCase>();
builder.Services.AddScoped<IRegisterUseCase, RegisterUseCase>();
builder.Services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();


JwtSettings? jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>() ?? throw new InvalidOperationException("Jwt settings are missing");
string connectionString = builder.Configuration.GetSection("ConnectionStrings")
    .GetSection("Default")
    .Get<string>() ?? throw new InvalidOperationException("DB connection is missing");

builder.Services.AddInfrastructure(jwtSettings, connectionString);

//token access settings
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Apply pending EF Core migrations at startup so the container is self-contained.
// Retries because Postgres may not be ready the moment the API process starts.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HomeInventoryDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    const int maxAttempts = 10;
    for (int attempt = 1; ; attempt++)
    {
        try
        {
            db.Database.Migrate();
            logger.LogInformation("Database migrations applied.");
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(ex, "Database not ready (attempt {Attempt}/{Max}), retrying in 3s...", attempt, maxAttempts);
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
