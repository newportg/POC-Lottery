using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;

namespace Services
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString, string tables)
        {
            if (services == null)
            {
                throw new Exception("Services are required");
            }

            services.AddSingleton<CloudStorageAccount>(provider =>
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                Console.WriteLine(connectionString);

                OptimizeTableConnection(storageAccount);
                return storageAccount;
            });

            services.AddSingleton<CloudTableClient>(provider =>
            {
                var tableClient = provider.GetRequiredService<CloudStorageAccount>().CreateCloudTableClient();
                return tableClient;
            });

            services.AddSingleton<IDictionary<string, CloudTable>>(provider =>
            {
                var atables = tables.Split(',');
                var client = provider.GetRequiredService<CloudTableClient>();
                var result = new Dictionary<string, CloudTable>();

                for (int i = 0; i < atables.Length; i++)
                {
                    result.Add(atables[i], client.GetTableReference(atables[i]));
                }

                return result;
            });

            return services;
        }

        private static void OptimizeTableConnection(CloudStorageAccount storageAccount)
        {
            var tableServicePoint = ServicePointManager.FindServicePoint(storageAccount.TableEndpoint);
            tableServicePoint.UseNagleAlgorithm = false;
            tableServicePoint.Expect100Continue = false;
            tableServicePoint.ConnectionLimit = 100;
        }
    }
}
