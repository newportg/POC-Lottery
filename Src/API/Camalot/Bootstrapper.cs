using Domain.Common.Mappings;
using Domain.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Camalot
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddCamalotServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new Exception("Services are required");
            }

            services.AddMapProfiles("Camalot");
            services.AddValidatorsFromAssembly(typeof(Bootstrapper).Assembly);
            services.AddSingleton<IDrawHistory, DrawHistory>();

            return services;
        }
    }
}
