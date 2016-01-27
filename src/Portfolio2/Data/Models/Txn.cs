using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio2.Data.Models
{
    public class Txn
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime TxnDate { get; set; }
        public int StockId { get; set; }

        public string Username { get; set; }
        public string TxnType { get; set; }
        public int Units { get; set; }
        public decimal Amount { get; set; }

        public Stock Stock { get; set; }
    }
}
