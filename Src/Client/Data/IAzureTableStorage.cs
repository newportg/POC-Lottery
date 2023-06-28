using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lottery.Data
{
    public interface IAzureTableStorage<T> where T : TableEntity, new()
    {
        Task Delete(string rowKey);
        Task<T> GetItem(string rowKey);
        Task<List<T>> GetList();
        Task Insert(T item);
        Task Update(T item);
    }
}
