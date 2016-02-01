using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Portfolio2.Data;
using Portfolio2.ViewModels;
using Microsoft.Data.Entity;
using Portfolio2.Data.Models;

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
            return View();

            var txns = (from t in _db.Txns.Include(t => t.Stock)
                        orderby t.Stock.Code, t.TxnDate
                        select t).ToList();
            var portfolio = ProcessPortfolio(txns);

            ViewBag.Count = portfolio.Count(p => p.Units > 0);
            ViewBag.CurrentValue = portfolio.Sum(p => p.CurrentValue);
            ViewBag.PurchaseValue = Math.Round(portfolio.Sum(p => p.PurchaseValue), 2);
            ViewBag.Profit = Math.Round(portfolio.Sum(p => p.Profit), 2);
            ViewBag.Growth = Math.Round(ViewBag.Profit / ViewBag.PurchaseValue * 100, 2);
            ViewBag.Dividends = portfolio.Sum(p => p.Dividends);
            ViewBag.IRR = CalculateIRR(txns, portfolio.Max(p => p.LastPriceDate), portfolio.Sum(p => p.CurrentValue)) * 100;
            ViewBag.AnnualisedReturn = Math.Round(Math.Pow((double)((ViewBag.Profit + ViewBag.Dividends) / ViewBag.PurchaseValue + 1), 1 / ((portfolio.Max(p => p.LastPriceDate) - txns.Min(t => t.TxnDate)).TotalDays / 365)) * 100 - 100, 2);


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
                    p.StockId = t.StockId;
                    result.Add(p);
                }
                p.Units += t.Units;

                // Calculate purchase values
                if (t.Units > 0)
                    p.PurchasePrice = (p.PurchaseValue - t.Amount) / p.Units;
                p.PurchaseValue = p.PurchasePrice * p.Units;

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
                    p2.Growth = p2.Profit / p2.PurchaseValue * 100;

                //Annualised Return
                if (p2.PurchaseValue != 0)
                {
                    double totReturn = (double)((p2.Profit + p2.Dividends) / p2.PurchaseValue + 1);
                    double totYears = ((p2.LastPriceDate - p2.Txns[0].TxnDate).TotalDays / 365);
                    p2.AnnualisedReturn = ((decimal)Math.Pow(totReturn, 1 / totYears) - 1) * 100;
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
            foreach(var t in txns)
            {
                npv -= t.Amount * (decimal)Math.Pow((double)(1 + rate), (asAt - t.TxnDate).TotalDays / 365);
            }
            return npv;
        }
    }
}
