using FluentValidation;
using Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ThunderBall
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddThunderBallServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new Exception("Services are required");
            }

            services.AddValidatorsFromAssembly(typeof(Bootstrapper).Assembly);
            services.AddSingleton<IThunderBallRepository, ThunderBallRepository>();

            return services;
        }
    }
}
