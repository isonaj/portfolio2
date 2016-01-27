using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio2.Data.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }
        public DateTime PriceDate { get; set; }
        public int StockId { get; set; }

        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }

        public Stock Stock { get; set; }
    }
}
