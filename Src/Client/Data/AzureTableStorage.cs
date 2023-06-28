using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lottery.Data
{
    public class AzureTableStorage<T> : IAzureTableStorage<T> where T : TableEntity, new()
    {
        private readonly AzureTableSettings settings;
        public AzureTableStorage(AzureTableSettings settings)
        {
            this.settings = settings;
        }
        public async Task<List<T>> GetList()
        {
            //Table  
            CloudTable table = await GetTableAsync();
            //Query  
            var query = new TableQuery<T>();
            var results = new List<T>();
            var continuationToken = default(TableContinuationToken);

            do
            {
                var queryResults = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);
            } while (continuationToken != null);

            return results;
        }
        public async Task<T> GetItem(string rowKey)
        {
            //Table  
            CloudTable table = await GetTableAsync();
            //Operation  
            TableOperation operation = TableOperation.Retrieve<T>("", rowKey);
            //Execute  
            TableResult result = await table.ExecuteAsync(operation);
            return (T)(dynamic)result.Result;
        }
        public async Task Insert(T item)
        {
            //Table  
            CloudTable table = await GetTableAsync();
            //Operation  
            TableOperation operation = TableOperation.Insert(item);
            //Execute  
            await table.ExecuteAsync(operation);
        }
        public async Task Update(T item)
        {
            //Table  
            CloudTable table = await GetTableAsync();
            //Operation  
            TableOperation operation = TableOperation.InsertOrReplace(item);
            //Execute  
            await table.ExecuteAsync(operation);
        }
        public async Task Delete(string rowKey)
        {
            //Item  
            T item = await GetItem(rowKey);
            //Table  
            CloudTable table = await GetTableAsync();
            //Operation  
            TableOperation operation = TableOperation.Delete(item);
            //Execute  
            await table.ExecuteAsync(operation);
        }
        private async Task<CloudTable> GetTableAsync()
        {
            //Account  
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.settings.ConnectionString);
            //Client  
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            //Table  
            CloudTable table = tableClient.GetTableReference(this.settings.TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
