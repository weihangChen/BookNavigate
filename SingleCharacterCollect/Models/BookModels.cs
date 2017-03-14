using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleCharacterCollect.Models
{
    public class Book
    {
        public Book(string bookName, string startUrl)
        {
            BookName = bookName;
            StartUrl = startUrl;
            History = new ProcessHistory();
        }
        public string BookName { get; set; }
        public string StartUrl { get; set; }
        public ProcessHistory History { get; set; }

    }
    public class ProcessHistory
    {
        public int ProcessedTotalPage { get; set; }

    }

    public class Books
    {
        public static List<Book> MyBooks = new List<Book>
        {

            new Book( "1","https://books.google.se/books?id=nH15DQAAQBAJ&printsec=frontcover&hl=sv&source=gbs_ge_summary_r&output=reader&pg=GBS.PT5")
            
        };

    }
}
