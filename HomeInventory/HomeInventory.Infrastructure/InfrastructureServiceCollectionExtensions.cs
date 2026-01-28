using HomeInventory.Application.Interfaces;
using HomeInventory.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            return services;
        }
    }

}
