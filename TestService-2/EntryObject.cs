using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestService_2
{
    class EntryObject
    {
        public String CustomerName { get; set; }

        public String BookName { get; set; }
        public int Quantity { get; set; }
        public String OrderDate { get; set; }



        public bool Equals(EntryObject other)
        {
            // Would still want to check for null etc. first.
            return this.CustomerName == other.CustomerName &&
                   this.BookName == other.BookName &&
                   this.Quantity == other.Quantity &&
                   this.OrderDate == other.OrderDate;
        }
    }
}
