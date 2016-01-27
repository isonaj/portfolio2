using Microsoft.Data.Entity;
using Portfolio2.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio2.Data
{
    public class PortfolioContext : DbContext
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Txn> Txns { get; set; }
        public DbSet<PriceHistory> PriceHistory { get; set; }

        /*
        static PortfolioContext()
        {
            Database.SetInitializer<PortfolioContext>(new CreateDatabaseIfNotExists<PortfolioContext>());

        }
        */
    }
}
