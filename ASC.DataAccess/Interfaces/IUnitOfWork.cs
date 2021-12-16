using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.DataAccess.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        Queue<Task<Action>> RollbackAction { get; set; }

        string ConnectionString { get; set; }
        IRepository<T> Responitory<T>() where T : TableEntity;
        void CommitTransaction();
    }
}
