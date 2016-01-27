using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Portfolio2.Data;
using Portfolio2.Data.Models;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Portfolio2.Controllers
{
    public class TxnsController : Controller
    {
        PortfolioContext _db;
        SelectList _typeList;

        public TxnsController(PortfolioContext db)
        {
            _db = db;
            _typeList = new SelectList(new List<string> { "Buy", "Sell", "Dividend" });
        }

        public IActionResult Index()
        {
            ViewBag.TypeList = _typeList;
            return View((from t in _db.Txns.Include(t => t.Stock)
                         orderby t.TxnDate, t.Stock.Code
                         select t).ToList());
        }

        public IActionResult Create()
        {
            ViewBag.TypeList = _typeList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Txn txn, string code)
        {
            if (ModelState.IsValid)
            {
                code = code.ToUpper();
                var stock = _db.Stocks.SingleOrDefault(s => s.Code == code);
                if (stock == null)
                {
                    stock = new Stock { Code = code };
                    txn.Stock = stock;
                    _db.Stocks.Add(stock);
                }
                else
                {
                    txn.StockId = stock.Id;
                }

                _db.Txns.Add(txn);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(txn);
        }
    }
}
