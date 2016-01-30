using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using System.IO;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Hosting;
using Portfolio2.Data;
using Portfolio2.Data.Models;
using System.Globalization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Portfolio2.Controllers
{
    public class UploadController : Controller
    {
        PortfolioContext _db;

        public UploadController(PortfolioContext db)
        {
            _db = db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ICollection<IFormFile> files)
        {
            //var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        await ProcessStockEasyFile(stream);
                    }
                    //var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    //await file.SaveAsAsync(Path.Combine(uploads, fileName));
                }
            }

            return View();
        }

        async Task ProcessStockEasyFile(Stream stream)
        {
            string line;
            using (StreamReader reader = new StreamReader(stream))
            {
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var tokens = line.Split(',');
                    //Ignore empty lines
                    if (tokens.Length > 1)
                    {
                        if (tokens.Length != 7)
                            throw new Exception("Invalid file format");

                        string code = tokens[0];
                        DateTime date = DateTime.ParseExact(tokens[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                        decimal open = Decimal.Parse(tokens[2]);
                        decimal high = Decimal.Parse(tokens[3]);
                        decimal low = Decimal.Parse(tokens[4]);
                        decimal close = Decimal.Parse(tokens[5]);
                        int volume = Int32.Parse(tokens[6]);

                        var stock = _db.Stocks.SingleOrDefault(s => s.Code == code);
                        if (stock != null)
                        {
                            var price = _db.PriceHistory.SingleOrDefault(ph => ph.StockId == stock.Id && ph.PriceDate == date);
                            if (price == null)
                            {
                                price = new PriceHistory { StockId = stock.Id, PriceDate = date };
                                _db.PriceHistory.Add(price);
                            }

                            price.Open = open;
                            price.High = high;
                            price.Low = low;
                            price.Close = close;
                            price.Volume = volume;                            
                        }
                    }
                }

                await _db.SaveChangesAsync();
            }
        }

        async Task ProcessTxnFile(string fileName)
        {

        }
    }
}
