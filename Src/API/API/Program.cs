using FluentValidation;
using Camalot;
using Domain;
using Domain.Common.Mappings;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Library.Azure.Odata;
using AutoMapper;
using System.ComponentModel;
using Domain.Models;

namespace API
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
                .ConfigureOpenApi()

                .ConfigureServices(services =>
                {
                    services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
                    {
                        var options = new OpenApiConfigurationOptions()
                        {
                            Info = new OpenApiInfo()
                            {
                                Version = "1.0.0",
                                Title = "Poc ThunderBall",
                                Description = "ThunderBall API",
                                TermsOfService = new Uri("https://www.knightfrank.com/legals"),
                                Contact = new OpenApiContact()
                                {
                                    Name = "Contact",
                                    Url = new Uri("https://www.knightfrank.com/contact"),
                                }
                            },
                            Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                            OpenApiVersion = OpenApiVersionType.V2,
                            IncludeRequestingHostName = true,
                            ForceHttps = false,
                            ForceHttp = false
                        };

                        return options;
                    });
                })
                .ConfigureAppConfiguration(config => config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables())
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddLogging();

                    services.AddMapProfiles("Domain");
                    services.AddValidatorsFromAssembly(typeof(Domain.Bootstrapper).Assembly);

                    services.AddDomainServices();
                    services.AddCamalotServices();
                    services.TryAddSingleton<System.Collections.Generic.Dictionary<string, ITableStore>>((container) =>
                    {
                        var act = Environment.GetEnvironmentVariable("AzureStorageAcct");
                        var key = Environment.GetEnvironmentVariable("AzureStorageKey");
                        var con = Environment.GetEnvironmentVariable("TableContainer");
                        var gus = Environment.GetEnvironmentVariable("GuessContainer");
                        var log = container.GetRequiredService<ILogger<TableStore>>();
                        Dictionary<string, ITableStore> dict = new Dictionary<string, ITableStore>(System.StringComparer.OrdinalIgnoreCase)
                        {
                            { con, new TableStore(act, key, con, log) },
                            { gus, new TableStore(act, key, gus, log) }
                        };

                        return dict;
                    });

                    services.AddSingleton<IHelper, Helper>();
                    services.AddSingleton<IGuessHelper, GuessHelper>();
                })
                .Build();

            host.Run();
        }
    }
}

