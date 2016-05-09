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

            result.CurrentValue = Math.Round(result.Items.Sum(p3 => p3.CurrentValue), 2);
            result.PurchaseValue = Math.Round(result.Items.Sum(p3 => p3.PurchaseValue), 2);
            result.Profit = Math.Round(result.Items.Sum(p3 => p3.Profit), 2);
            result.Growth = Math.Round(result.Profit / result.PurchaseValue * 100, 2);
            result.Dividends = result.Items.Sum(p3 => p3.Dividends);
            result.IRR = CalculateIRR(txns, result.Items.Max(p3 => p3.LastPriceDate), result.Items.Sum(p3 => p3.CurrentValue)) * 100;
            result.AnnualisedReturn = (decimal)Math.Round(Math.Pow((double)((result.Profit + result.Dividends) / result.PurchaseValue + 1), 1 / ((result.Items.Max(p3 => p3.LastPriceDate) - txns.Min(t => t.TxnDate)).TotalDays / 365)) * 100 - 100, 2);

            return result;
        }

        /*
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

        //http://www.codeproject.com/Tips/461049/Internal-Rate-of-Return-IRR-Calculation
#define LOW_RATE 0.01
#define HIGH_RATE 0.5
#define MAX_ITERATION 1000
#define PRECISION_REQ 0.00000001
        decimal CalculateIRR(List<Txn> txns, DateTime currentDate, decimal currentValue)
        {
        int i = 0,j = 0;
 double m = 0.0;
 double old = 0.00;
 double new = 0.00;
 double oldguessRate = LOW_RATE;
 double newguessRate = LOW_RATE;
 double guessRate = LOW_RATE;
 double lowGuessRate = LOW_RATE;
 double highGuessRate = HIGH_RATE;
 double npv = 0.0;
 double denom = 0.0;
            Func<decimal, decimal> f = (x) => CalculateNPV(txns, currentDate, x);
            for (var i = 0; i < 100; i++)
            {
                var npv = f(guessRate);
/* Stop checking once the required precision is achieved 
  if((npv > 0) && (npv<PRECISION_REQ))
   return guessRate;
  if(old == 0)
   old = npv;
  else
   old = new;
  new = npv;
  if(i > 0)
  {
   if(old< new)
   {
    if(old< 0 && new < 0)
     highGuessRate = newguessRate;
    else
     lowGuessRate = newguessRate;
   }
   else
   {
    if(old > 0 && new > 0)
     lowGuessRate = newguessRate;
    else
     highGuessRate = newguessRate;
   }
  }
  oldguessRate = guessRate;
  guessRate = (lowGuessRate + highGuessRate) / 2;
  newguessRate = guessRate;            }
            return 999;
        }
        */

        // Use Brent's method for calculating IRR
        // https://en.wikipedia.org/wiki/Brent%27s_method
        // http://apps.nrbook.com/empanel/index.html#pg=454
        decimal CalculateIRR(List<Txn> txns, DateTime currentDate, decimal currentValue)
        {
            var tol = 0.01M; // Tolerance
            Func<decimal, decimal> f = (x) => CalculateNPV(txns, currentDate, x);
            decimal a = -100M;  // min
            decimal b = 100M;   // max
            decimal c = 100M;
            var fa = f(a);
            var fb = f(b);
            decimal d = 0, e = 0, fc, p, q, r, s, xm, tol1;

            // Check if root is bracketed
            if (fa * fb >= 0)
                return 999M;
            fc = fb;
            for (var i = 0; i < 100; i++)
            {
                if ((fb > 0 && fc > 0) || (fb < 0 && fc < 0))
                {
                    c = a;
                    fc = fa;
                    d = b - a;
                    e = d;
                }
                if (Math.Abs(fc) > Math.Abs(fb))
                {
                    a = b;
                    b = c;
                    c = a;
                    fa = fb;
                    fb = fc;
                    fc = fa;
                }
                tol1 = 0.000002M * Math.Abs(b) + 0.5M * tol;
                xm = 0.5M * (c - b);
                if (Math.Abs(xm) <= tol1 || fb == 0)
                    return b;
                if (Math.Abs(e) >= tol1 && Math.Abs(fa) > Math.Abs(fb))
                {
                    s = fb / fa;
                    if (a == c)
                    {
                        p = 2 * xm * s;
                        q = 1 - s;
                    }
                    else
                    {
                        q = fa / fc;
                        r = fb / fc;
                        p = s * (2 * xm * q * (q - r) - (b - a) * (r - 1));
                        q = (q - 1) * (r - 1) * (s - 1);
                    }
                    if (p > 0)
                        q = -q;
                    p = Math.Abs(p);
                    var min1 = 3 * xm * q - Math.Abs(tol1 * q);
                    var min2 = Math.Abs(e * q);
                    if (2 * p < (min1 < min2 ? min1: min2))
                    {
                        e = d;
                        d = p / q;
                    }
                    else
                    {
                        d = xm;
                        e = d;
                    }
                }
                else
                {
                    d = xm;
                    e = d;
                }
                a = b;
                fa = fb;
                if (Math.Abs(d) > tol1)
                    b += d;
                else
                {
                    b += tol1 * Math.Sign(xm); 
                    fb = f(b);
                }
            }
            return 999M;
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
