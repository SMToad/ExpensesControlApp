using ExpensesControlApp.Data;
using ExpensesControlApp.Models;
using ExpensesControlApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Controllers
{
    public class ExpenseEntryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ExpenseEntryController(ApplicationDbContext db)
        {
            _db = db;
        }
        //GET-Index
        public IActionResult Index(string? timeSpan, string? sortOrder)
        {
            var expenseList = (from exp in _db.Expenses
                               join expEntry in _db.ExpenseEntries
                               on exp.Id equals expEntry.ExpenseId
                               select new
                               {
                                   Id = expEntry.EntryId,
                                   //ExpenseName = exp.ExpenseName,
                                   //Amount = exp.Amount,
                                   Date = expEntry.Date,
                                   Expense = expEntry.Expense,
                                   ExpenseId = exp.Id
                               }).ToList()
                               .Select(exp => new ExpenseEntry()
                               {
                                   EntryId = exp.Id,
                                   ExpenseId = exp.ExpenseId,
                                   Expense = exp.Expense,
                                   //ExpenseName = exp.ExpenseName,
                                   //Amount = exp.Amount,
                                   Date = exp.Date
                               });
            LimitParam limitParams = new LimitParam(_db.Params);
            TempData["TimeSpan"] = timeSpan ?? "total";
            TempData["SortOrder"] = sortOrder ?? "date_desc";
            TimeSpanOption timeSpanOption;

            ViewBag.DateSortParm = sortOrder == "date_desc" ? "date" : "date_desc";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.AmountSortParm = sortOrder == "amount" ? "amount_desc" : "amount";
            
            switch (timeSpan)
            {
                case "today":
                    expenseList = expenseList.Where(o => o.Date.Date == DateTime.Today);
                    timeSpanOption = new Today();
                    break;
                case "week":
                    expenseList = expenseList.Where(o => ISOWeek.GetWeekOfYear(o.Date) == ISOWeek.GetWeekOfYear(DateTime.Today));
                    timeSpanOption = new Week();
                    break;
                case "month":
                    expenseList = expenseList.Where(o => o.Date.Month == DateTime.Today.Month);
                    timeSpanOption = new Month();
                    break;
                default:
                    timeSpanOption = new Total();
                    break;
            }
            TempData["LimitAmount"] = timeSpanOption.GetLimit(limitParams);
            TempData["LimitTimeSpan"] = timeSpanOption.Name;
           
            switch (sortOrder)
            {
                case "name":
                    expenseList = expenseList.OrderBy(o => o.Expense.ExpenseName).ToList();
                    break;
                case "name_desc":
                    expenseList = expenseList.OrderByDescending(o => o.Expense.ExpenseName).ToList();
                    break;
                case "amount":
                    expenseList = expenseList.OrderBy(o => o.Expense.Amount).ToList();
                    break;
                case "amount_desc":
                    expenseList = expenseList.OrderByDescending(o => o.Expense.Amount).ToList();
                    break;
                case "date":
                    expenseList = expenseList.OrderBy(o => o.Date).ToList();
                    break;
                default:
                    expenseList = expenseList.OrderByDescending(o => o.Date).ToList();
                    break;
            }
            
            return View(new ExpenseEntryViewModel() { ExpenseEntries = expenseList, LimitParam = limitParams});
        }
       
        public IActionResult Create(ExpenseEntry expenseEntry)
        {
            Expense expense = _db.Expenses.Find(expenseEntry.ExpenseId);
            expenseEntry.Expense = expense;
            return View(expenseEntry);
        }
        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost(ExpenseEntry expenseEntry)
        {
            Expense expense = _db.Expenses.Find(expenseEntry.ExpenseId);
            expenseEntry.Expense = expense;
            //if (ModelState.IsValid)
            //{
            _db.ExpenseEntries.AddIfNotExists(expenseEntry, x => x.Date == expenseEntry.Date && x.ExpenseId == expenseEntry.ExpenseId);
                _db.SaveChanges();
                return RedirectToAction("Index");
            //}
            //return View(expenseEntry);
        }
        // GET Update
        public IActionResult Update(int? entryId)
        {
            if (entryId == null || entryId == 0)
                return NotFound();
            var expenseEntry = _db.ExpenseEntries.Find(entryId);
            if (expenseEntry == null)
                return NotFound();
            var expense = _db.Expenses.Find(expenseEntry.ExpenseId);
            expenseEntry.Expense = expense;
            expenseEntry.ExpenseId = expense.Id;
            return View(expenseEntry);//new Temp() { Expense = expense, ExpenseDate = expenseDate });

        }
        // POST Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ExpenseEntry expenseEntry)
        {
            //server side validation error handling
            if (ModelState.IsValid)
            {
                var expense = _db.Expenses.Find(expenseEntry.ExpenseId);
                if (!_db.ExpenseEntries.Any(o => o.ExpenseId == expense.Id))
                        _db.Expenses.Update(expense);
                    else
                    {
                        
                        expense.Id = 0;
                        expenseEntry.ExpenseId = 0;
                        _db.Expenses.AddIfNotExists<Expense>(expense, x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                    }
                    _db.SaveChanges();
                if (expenseEntry.ExpenseId == 0)
                {
                    expenseEntry.Expense = _db.Expenses.FirstOrDefault(x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                }
                _db.ExpenseEntries.Update(expenseEntry);
                _db.Expenses.Update(expenseEntry.Expense);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(expenseEntry);
        }
        // POST Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string entryId)
        {
            var expenseEntry = _db.ExpenseEntries.Find(Convert.ToInt32(entryId));
            if (expenseEntry is null)
            {
                return NotFound();
            }
            _db.ExpenseEntries.Remove(expenseEntry);
            _db.SaveChanges();
            int expenseId = expenseEntry.ExpenseId;
            if (!_db.ExpenseEntries.Any(o => o.ExpenseId == expenseId))
            {
                var expense = _db.Expenses.Find(expenseId);
                _db.Expenses.Remove(expense);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

    }
}
