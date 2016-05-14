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

            foreach (var p in portfolio.Items)
                p.Txns = null;

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

        Portfolio ProcessPortfolio(List<Data.Models.Txn> txns)
        {
            var result = new Portfolio();
            PortfolioItem p = null;
            foreach (var t in txns)
            {
                if (p == null || p.Code != t.Stock.Code)
                {
                    p = new PortfolioItem();
                    p.Code = t.Stock.Code;
                    p.StockId = t.StockId;
                    result.Items.Add(p);
                }
                p.Units += t.Units;

                // Calculate purchase values
                if (t.Units > 0)
                    p.PurchasePrice = Math.Round((p.PurchaseValue - t.Amount) / p.Units, 3);
                p.PurchaseValue = Math.Round(p.PurchasePrice * p.Units, 2);

                if (t.TxnType == "Dividend")
                    p.Dividends += t.Amount;

                if (t.TxnType == "Sell")
                    p.RealisedProfit += t.Amount + (p.PurchasePrice * t.Units);

                p.Txns.Add(t);
            }

            foreach (var p2 in result.Items)
            {
                // Get last price
                var price = _db.PriceHistory
                    .Where(ph => ph.StockId == p2.StockId)
                    .OrderByDescending(ph => ph.PriceDate)
                    .FirstOrDefault();
                p2.LastPrice = price == null ? 0 : price.Close;
                p2.LastPriceDate = price == null ? DateTime.Today : price.PriceDate;

                p2.CurrentValue = p2.Units * p2.LastPrice;
                p2.UnrealisedProfit = p2.CurrentValue - p2.PurchaseValue;
                if (p2.PurchaseValue != 0)
                    p2.Growth = Math.Round(p2.UnrealisedProfit / p2.PurchaseValue * 100, 1);

                //Annualised Return
                if (p2.PurchaseValue != 0)
                {
                    double totReturn = (double)((p2.UnrealisedProfit + p2.Dividends) / p2.PurchaseValue + 1);
                    double totYears = ((p2.LastPriceDate - p2.Txns[0].TxnDate).TotalDays / 365);
                    p2.AnnualisedReturn = Math.Round(((decimal)Math.Pow(totReturn, 1 / totYears) - 1) * 100, 1);
                }
                p2.IRR = CalculateIRR(p2.Txns, p2.LastPriceDate, p2.CurrentValue) * 100;
            }

            result.CurrentValue = Math.Round(result.Items.Sum(p3 => p3.CurrentValue), 2);
            result.PurchaseValue = Math.Round(result.Items.Sum(p3 => p3.PurchaseValue), 2);
            result.UnrealisedProfit = Math.Round(result.Items.Sum(p3 => p3.UnrealisedProfit), 2);
            result.RealisedProfit = Math.Round(result.Items.Sum(p3 => p3.RealisedProfit), 2);
            result.Growth = Math.Round(result.UnrealisedProfit / result.PurchaseValue * 100, 2);
            result.Dividends = result.Items.Sum(p3 => p3.Dividends);
            result.IRR = CalculateIRR(txns, result.Items.Max(p3 => p3.LastPriceDate), result.Items.Sum(p3 => p3.CurrentValue)) * 100;
            result.AnnualisedReturn = (decimal)Math.Round(Math.Pow((double)((result.UnrealisedProfit + result.Dividends + result.RealisedProfit) / result.PurchaseValue + 1), 1 / ((result.Items.Max(p3 => p3.LastPriceDate) - txns.Min(t => t.TxnDate)).TotalDays / 365)) * 100 - 100, 2);

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

                    if (irr > 10)
                        return irr;
                }
            }
            else
            {
                while (currentValue < npv)
                {
                    irr -= 0.01M;
                    npv = CalculateNPV(txns, currentDate, irr);
                    if (irr < -10)
                        return irr;
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
