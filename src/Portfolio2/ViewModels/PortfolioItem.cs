using Portfolio2.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio2.ViewModels
{
    public class PortfolioItem
    {
        public int StockId { get; set; }
        public string Code { get; set; }
        public int Units { get; set; }
        public decimal LastPrice { get; set; }
        public DateTime LastPriceDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal PurchaseValue { get; set; }
        public decimal Profit { get; set; }
        public decimal Growth { get; set; }
        public decimal Dividends { get; set; }
        public decimal TotalYield { get; set; }
        public decimal AvgYield { get; set; }
        public decimal IRR { get; set; }
        public decimal AnnualisedReturn { get; set; }

        public List<Txn> Txns { get; set; }

        public PortfolioItem()
        {
            Txns = new List<Txn>();
        }
    }
}
