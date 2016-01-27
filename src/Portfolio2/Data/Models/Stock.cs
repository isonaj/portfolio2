using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.ComponentModel.DataAnnotations;

namespace Portfolio2.Data.Models
{
    public class Stock
    {
        public int Id { get; set; }
        //[Index(IsUnique= true)]
        //[Length]
        public string Code { get; set; }

        public List<Txn> Txns { get; set; }
        public List<PriceHistory> PriceHistory { get; set; }

        public Stock()
        {
            Txns = new List<Txn>();
            PriceHistory = new List<PriceHistory>();
        }
    }
}
