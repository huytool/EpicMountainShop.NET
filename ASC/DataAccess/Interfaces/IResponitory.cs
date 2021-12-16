using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.DataAccess.Interfaces
{
    public interface IResponitory<T> where T : TableEntity
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeteleAsync(T entity);
        Task<T> FindAsync(string partitionKey,string rowKey);
        Task<IEnumerable<T>> FindAllByPartitionKeyAsync(string partitionKey);
        Task<IEnumerable<T>> FindAllAsync(string partitionKey);
        Task CreateTableAsync();
    }
}
