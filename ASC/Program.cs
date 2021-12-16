using ASC.DataAccess;
using ASC.Modal;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace ASC
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var _unitOfWork = new UnitOfWork("UseDevelopmentStorage=true;"))
            {
                var bookRespository = _unitOfWork.Responitory<Book>();
                await bookRespository.CreateTableAsync();
                Book book = new Book()
                {
                    Author = "Ramiii",
                    BookName = "ASP.NET Core with Azure",
                    Punlisher = "Apress"
                };
                book.BookId = 1;
                book.RowKey = book.BookId.ToString();
                book.PartitionKey = book.Punlisher;
                var data = await bookRespository.AddAsync(book);
                Console.WriteLine(data);
                _unitOfWork.CommitTransaction();
            }

            using (var _unitOfWork = new UnitOfWork("UseDevelopmentStorage=true;"))
            {
                var bookRespository = _unitOfWork.Responitory<Book>();
                await bookRespository.CreateTableAsync();

            
                var data = await bookRespository.FindAsync("Apress","1");
                Console.WriteLine(data);
             
                data.Author = "Rami Vemula";
                var updatedData = await bookRespository.UpdateAsync(data);
                   Console.WriteLine(updatedData);
                _unitOfWork.CommitTransaction();
            }

            using (var _unitOfWork = new UnitOfWork("UseDevelopmentStorage=true;"))
            {
                var bookRespository = _unitOfWork.Responitory<Book>();
                await bookRespository.CreateTableAsync();
                var data = await bookRespository.FindAsync("Apress", "1");
                Console.WriteLine(data);
                await bookRespository.DeteleAsync(data);
     
                Console.WriteLine("Delete");
                _unitOfWork.CommitTransaction();
            }


            Console.WriteLine();
        }
       

    }
}
