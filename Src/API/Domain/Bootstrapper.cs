using Domain.Common.Mappings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Domain
{
    public static class Bootstrapper
    {
        //static ThunderBallRules rules = new ThunderBallRules();

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new Exception("Services are required");
            }

            services.AddMapProfiles("Domain");
            //services.AddMediatR(Assembly.GetExecutingAssembly()); -- pre 12.01
            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Behaviours
            services.AddValidatorsFromAssembly(typeof(Bootstrapper).Assembly);
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services;
        }
    }
}
