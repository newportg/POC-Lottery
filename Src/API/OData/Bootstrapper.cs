using AutoMapper;
using FluentValidation;
using Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace OData
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddODataServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new Exception("Services are required");
            }

            var tableStorageUrl = Environment.GetEnvironmentVariable("TableStorageUrl");
            var thunderBallSasKey = Environment.GetEnvironmentVariable("ThunderBallSasKey");
            var thunderBallStorageTableName = Environment.GetEnvironmentVariable("ThunderBallStorageTableName");
            var predictionSasKey = Environment.GetEnvironmentVariable("PredictionSasKey");
            var predictionStorageTableName = Environment.GetEnvironmentVariable("PredictionStorageTableName");

            services.AddValidatorsFromAssembly(typeof(Bootstrapper).Assembly);
            services.AddSingleton<IThunderBallTable, ThunderBallTable>(provider =>
            {
                var ts = new TableStore(tableStorageUrl, thunderBallSasKey, thunderBallStorageTableName);
                return new ThunderBallTable(provider.GetRequiredService<IMapper>(), provider.GetRequiredService<ILogger<ThunderBallTable>>(), provider.GetRequiredService<IValidator<Models.ThunderBallEntity>>(), ts);
            });

            //services.AddSingleton<IPredictionTable, PredictionTable>(provider =>
            //{
            //    var ts = new TableStore(tableStorageUrl, predictionSasKey, predictionStorageTableName);
            //    return new PredictionTable( provider.GetRequiredService<IMapper>(), provider.GetRequiredService<ILogger<PredictionTable>>(), provider.GetRequiredService<IValidator<Models.PredictionEntity>>(), ts);
            //});

            return services;
        }
    }
}
