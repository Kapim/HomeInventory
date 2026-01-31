using HomeInventory.Application.Interfaces;
using HomeInventory.Infrastructure.InMemory;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Repositories;
using HomeInventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, JwtSettings jwtSettings, string connectionString)
        {
            services.AddScoped<IUserRepository, InMemoryUserRepository>();
            services.AddSingleton<ITokenService>(sp => new TokenService(jwtSettings));
            services.AddDbContext<HomeInventoryDbContext>(opt => opt.UseNpgsql(connectionString));
            services.AddScoped<ILocationRepository, EfLocationRepository>();
            return services;
        }
    }

}
