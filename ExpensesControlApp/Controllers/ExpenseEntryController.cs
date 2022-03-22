using ExpensesControlApp.Data;
using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;
using ExpensesControlApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

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
            // expense list for the ViewModel
            var expenseList = (from exp in _db.Expenses
                               join expEntry in _db.ExpenseEntries
                               on exp.Id equals expEntry.ExpenseId
                               select new
                               {
                                   Id = expEntry.EntryId,
                                   ExpenseId = exp.Id,
                                   ExpenseName = exp.ExpenseName,
                                   Amount = exp.Amount,
                                   Date = expEntry.Date
                                  
                               }).ToList()
                               .Select(entry => new ExpenseEntry()
                               {
                                   EntryId = entry.Id,
                                   ExpenseId = entry.ExpenseId,
                                   ExpenseName = entry.ExpenseName,
                                   Amount = entry.Amount,
                                   Date = entry.Date
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
                    expenseList = expenseList.OrderBy(o => o.ExpenseName).ToList();
                    break;
                case "name_desc":
                    expenseList = expenseList.OrderByDescending(o => o.ExpenseName).ToList();
                    break;
                case "amount":
                    expenseList = expenseList.OrderBy(o => o.Amount).ToList();
                    break;
                case "amount_desc":
                    expenseList = expenseList.OrderByDescending(o => o.Amount).ToList();
                    break;
                case "date":
                    expenseList = expenseList.OrderBy(o => o.Date).ToList();
                    break;
                default:
                    expenseList = expenseList.OrderByDescending(o => o.Date).ToList();
                    break;
            }

            return View(new ExpenseEntryViewModel() { ExpenseEntries = expenseList, LimitParam = limitParams });
        }

        public IActionResult Create()
        {
            return View();
        }
        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExpenseEntry expenseEntry)
        {
            // Add Expense if it doesn't exist already
            var expense = new Expense() { ExpenseName = expenseEntry.ExpenseName, Amount = expenseEntry.Amount };
            Expression<Func<Expense, bool>> predicate = (x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
            _db.Expenses.AddIfNotExists<Expense>(expense, predicate);
            _db.SaveChanges();
            // Add ExpenseEntry if it doesn't exist already
            var expenseInDb = _db.Expenses.FirstOrDefault(predicate);
            expenseEntry.Expense = expenseInDb;
            expenseEntry.ExpenseId = expenseInDb.Id;
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
            expenseEntry.ExpenseId = expense.Id;
            expenseEntry.ExpenseName = expense.ExpenseName;
            expenseEntry.Amount = expense.Amount;
            return View(expenseEntry);

        }
        // POST Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ExpenseEntry expenseEntry)
        {
            //if (ModelState.IsValid)
            //{
            var expense = new Expense()
            {
                Id = expenseEntry.ExpenseId,
                ExpenseName = expenseEntry.ExpenseName,
                Amount = expenseEntry.Amount
            };
            var expenseInDb = _db.Expenses.Find(expense.Id);
            // if Expense entity was modified
            if (expenseInDb.ExpenseName != expense.ExpenseName || expenseInDb.Amount != expense.Amount)
            {
                // update if this Expense entity isn't connected to other ExpenseEntry entities
                if (_db.ExpenseEntries.Where(o => o.ExpenseId == expense.Id).Count() == 1)
                    _db.Expenses.Update(expense);
                else
                {
                    // else add a new Expense entity if it doesn't already exist
                    expense.Id = 0;
                    expenseEntry.ExpenseId = 0;
                    _db.Expenses.AddIfNotExists<Expense>(expense, x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                }
                _db.SaveChanges();
                // get new ExpenseId if it was modified 
                if (expenseEntry.ExpenseId == 0)
                {
                    expense = _db.Expenses.FirstOrDefault(x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                    expenseEntry.ExpenseId = expense.Id;
                }
            }
            _db.Entry(expenseInDb).State = EntityState.Detached;
            // update ExpenseEntry entity
            expenseEntry.Expense = expense;
            _db.ExpenseEntries.Update(expenseEntry);
            _db.SaveChanges();
            return RedirectToAction("Index");
            //}
            //return View(expenseEntry);
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
