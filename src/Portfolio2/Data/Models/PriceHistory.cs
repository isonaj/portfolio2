using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio2.Data.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }
        public DateTime PriceDate { get; set; }
        public int StockId { get; set; }

        [Column(TypeName="decimal(18,3)")]
        public decimal Open { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal High { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal Low { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal Close { get; set; }
        public int Volume { get; set; }

        public Stock Stock { get; set; }
    }
}
