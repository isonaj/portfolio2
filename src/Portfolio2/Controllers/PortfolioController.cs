using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Portfolio2.Data;
using Portfolio2.ViewModels;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Portfolio2.Controllers
{
    public class PortfolioController : Controller
    {
        PortfolioContext _db;
        public PortfolioController(PortfolioContext db)
        {
            _db = db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var portfolio = ProcessPortfolio((from t in _db.Txns.Include(t => t.Stock)
                                              orderby t.Stock.Code, t.TxnDate
                                              select t).ToList());
            return View(portfolio);
        }

        List<Portfolio> ProcessPortfolio(List<Data.Models.Txn> txns)
        {
            var result = new List<Portfolio>();
            Portfolio p = null;
            foreach (var t in txns)
            {
                if (p == null || p.Code != t.Stock.Code)
                {
                    p = new Portfolio();
                    p.Code = t.Stock.Code;
                    result.Add(p);
                }
                p.Units += t.Units;
                if (t.TxnType == "Dividend")
                    p.Dividends += t.Amount;
            }
            return result;
        }
    }
}
