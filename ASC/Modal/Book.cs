using ASC.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Modal
{
    class Book:BaseEntity, IAuditTracker
    {
        public Book() { }
        public Book(int bookid,string publisher)
        {
            this.RowKey = bookid.ToString();
            this.PartitionKey = publisher;
        }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string Punlisher { get; set; }
    }
}
