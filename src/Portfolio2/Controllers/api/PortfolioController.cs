using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Portfolio2.ViewModels;
using Portfolio2.Data;
using Microsoft.Data.Entity;
using Portfolio2.Data.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Portfolio2.Controllers.api
{
    [Route("api/[controller]")]
    public class PortfolioController : Controller
    {
        PortfolioContext _db;
        public PortfolioController(PortfolioContext db)
        {
            _db = db;
        }

        // GET: api/Portfolio
        [HttpGet]
        public IActionResult Get()
        {
            var txns = (from t in _db.Txns.Include(t => t.Stock)
                        orderby t.Stock.Code, t.TxnDate
                        select t).ToList();
            var portfolio = ProcessPortfolio(txns);

            foreach (var p in portfolio)
                p.Txns = null;

            var p2 = new List<Portfolio>
            {
                new Portfolio { Code = "ALU",
                    AnnualisedReturn = 5.80887292179400M,
                    AvgYield = 0,
                    CurrentValue = 5170.92M,
                    Dividends = 42.48M,
                    Growth = 3.0324404779695261550232827800M,
                    IRR = 9.00M,
                    LastPrice = 4.92M,
                    LastPriceDate = new DateTime(2016, 1, 29),
                    Profit = 152.1900000000000000000000000M,
                    PurchasePrice = 4.7751950523311132254995242626M,
                    PurchaseValue = 5018.7300000000000000000000000M,
                    StockId = 47,
                    TotalYield = 0,
                    Units = 1051,
                    Txns = new List<Txn>
                    {
                        new Txn { TxnType = "Buy", Units = 100, Amount = -3000, Stock = new Stock { Code = "ALU" } }
                    }
                }
            };
            return new ObjectResult(portfolio);
        }

        /*
        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */

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
                    p.StockId = t.StockId;
                    result.Add(p);
                }
                p.Units += t.Units;

                // Calculate purchase values
                if (t.Units > 0)
                    p.PurchasePrice = Math.Round((p.PurchaseValue - t.Amount) / p.Units, 3);
                p.PurchaseValue = Math.Round(p.PurchasePrice * p.Units, 2);

                if (t.TxnType == "Dividend")
                    p.Dividends += t.Amount;

                p.Txns.Add(t);
            }

            foreach (var p2 in result)
            {
                // Get last price
                var price = _db.PriceHistory
                    .Where(ph => ph.StockId == p2.StockId)
                    .OrderByDescending(ph => ph.PriceDate)
                    .FirstOrDefault();
                p2.LastPrice = price == null ? 0 : price.Close;
                p2.LastPriceDate = price == null ? DateTime.Today : price.PriceDate;

                p2.CurrentValue = p2.Units * p2.LastPrice;
                p2.Profit = p2.CurrentValue - p2.PurchaseValue;
                if (p2.PurchaseValue != 0)
                    p2.Growth = Math.Round(p2.Profit / p2.PurchaseValue * 100, 1);

                //Annualised Return
                if (p2.PurchaseValue != 0)
                {
                    double totReturn = (double)((p2.Profit + p2.Dividends) / p2.PurchaseValue + 1);
                    double totYears = ((p2.LastPriceDate - p2.Txns[0].TxnDate).TotalDays / 365);
                    p2.AnnualisedReturn = Math.Round(((decimal)Math.Pow(totReturn, 1 / totYears) - 1) * 100, 1);
                }
                p2.IRR = CalculateIRR(p2.Txns, p2.LastPriceDate, p2.CurrentValue) * 100;
            }

            return result;
        }

        decimal CalculateIRR(List<Txn> txns, DateTime currentDate, decimal currentValue)
        {
            decimal irr = 0M;
            decimal npv = CalculateNPV(txns, currentDate, irr);
            if (currentValue > npv)
            {
                while (currentValue > npv)
                {
                    irr += 0.01M;
                    npv = CalculateNPV(txns, currentDate, irr);
                }
            }
            else
            {
                while (currentValue < npv)
                {
                    irr -= 0.01M;
                    npv = CalculateNPV(txns, currentDate, irr);
                }
            }
            return irr;
        }

        decimal CalculateNPV(List<Txn> txns, DateTime asAt, decimal rate)
        {
            decimal npv = 0M;
            foreach (var t in txns)
            {
                npv -= t.Amount * (decimal)Math.Pow((double)(1 + rate), (asAt - t.TxnDate).TotalDays / 365);
            }
            return npv;
        }
    }
}
