using ExpensesControlApp.Data;
using ExpensesControlApp.Models;
using ExpensesControlApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesControlApp.Controllers
{
    public class LimitController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LimitController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            Limit limit = new Limit(_db.Props);
            return View(limit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Limit limit)
        {
            if (ModelState.IsValid)
            {
                var amount = _db.Props.SingleOrDefault(p => p.Key == "limitAmount"); 
                amount.Value= limit.Amount.ToString();
                var timeSpan = _db.Props.SingleOrDefault(p => p.Key == "limitTimeSpan");
                timeSpan.Value = ((int)limit.TimeSpan).ToString();
                _db.SaveChanges();
                return RedirectToAction("Index", "ExpenseManager");
            }
            return View(limit);
        }
    }
}
