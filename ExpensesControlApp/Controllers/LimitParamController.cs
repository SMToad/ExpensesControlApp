using ExpensesControlApp.Data;
using ExpensesControlApp.Models;
using ExpensesControlApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesControlApp.Controllers
{
    public class LimitParamController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LimitParamController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            LimitParam limitParams = new LimitParam(_db.Params);
            return View(limitParams);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LimitParam obj)
        {
            if (ModelState.IsValid)
            {
                var amount = _db.Params.SingleOrDefault(p => p.Key == "limitAmount"); 
                amount.Value= obj.Amount.ToString();
                var timeSpan = _db.Params.SingleOrDefault(p => p.Key == "limitTimeSpan");
                timeSpan.Value = ((int)obj.TimeSpan).ToString();
                _db.SaveChanges();
                return RedirectToAction("Index", "ExpenseManager");
            }
            return View(obj);
        }
    }
}
