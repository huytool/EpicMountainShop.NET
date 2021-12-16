using ASC.DataAccess.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.DataAccess
{
    public class UnitOfWork :IUnitOfWork
    {
        private bool complete;
        private Dictionary<string, object> _respositories;
       
        public Queue<Task<Action>> RollbackAction { get ; set; }
        public string ConnectionString { get ; set; }
        public UnitOfWork(string connectionString)
        {
            ConnectionString = connectionString;
            RollbackAction = new Queue<Task<Action>>();
        }
        public void CommitTransaction()
        {
            complete = true;
        }
        public void RollbackTransaction()
        {
            while(RollbackAction.Count > 0)
            {
                var undoAction = RollbackAction.Dequeue();
                undoAction.Result();
            }
        }
        ~UnitOfWork()
        {
            Dispose(false);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (!complete)
                    {
                        RollbackTransaction();
                    }
                }
                finally
                {
                    RollbackAction.Clear();
                }
            }
            complete = false;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
      
       

        public IRepository<T> Responitory<T>() where T : TableEntity
        {
            if (_respositories == null)
                _respositories = new Dictionary<string, object>();
             var type = typeof(T).Name;
              if (_respositories.ContainsKey(type)) return (IRepository<T>)_respositories[type];
              var respositoryType = typeof(Respository<>);
              var respositoryInstance = Activator.CreateInstance(respositoryType.MakeGenericType(typeof(T)), this);
              _respositories.Add(type, respositoryInstance);
              return (IRepository<T>)_respositories[type];
            
        }

       
    }
}
