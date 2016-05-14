using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio2.ViewModels
{
    public class Portfolio
    {
        public decimal CurrentValue { get; set; }
        public decimal PurchaseValue { get; set; }
        public decimal UnrealisedProfit { get; set; }
        public decimal RealisedProfit { get; set; }
        public decimal Growth { get; set; }
        public decimal Dividends { get; set; }
        public decimal IRR { get; set; }
        public decimal AnnualisedReturn { get; set; }

        public List<PortfolioItem> Items { get; set; }

        public Portfolio()
        {
            Items = new List<PortfolioItem>();
        }
    }
}
