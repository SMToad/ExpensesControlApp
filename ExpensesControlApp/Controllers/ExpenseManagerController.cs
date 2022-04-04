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
    public class ExpenseManagerController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ExpenseManagerController(ApplicationDbContext db)
        {
            _db = db;
        }
        //GET-Index
        public IActionResult Index(string? timeSpan, string? sortOrder)
        {
            // expense list for the ViewModel
            var expEntryList = (from exp in _db.Expenses
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
                               .Select(entry => new ExpenseEntryVM()
                               {
                                   EntryId = entry.Id,
                                   ExpenseId = entry.ExpenseId,
                                   ExpenseName = entry.ExpenseName,
                                   Amount = entry.Amount,
                                   Date = entry.Date
                               });
            var regExpList = (from exp in _db.Expenses
                              join regExp in _db.RegularExpenses
                              on exp.Id equals regExp.ExpenseId
                              select new
                              {
                                  Id = regExp.RegularExpenseId,
                                  ExpenseId = exp.Id,
                                  ExpenseName = exp.ExpenseName,
                                  Amount = exp.Amount,
                                  TimeSpan = regExp.TimeSpan
                                  
                              }).ToList()
                               .Select(regExp => new RegularExpenseVM()
                               {
                                   RegularExpenseId = regExp.Id,
                                   ExpenseId = regExp.ExpenseId,
                                   ExpenseName = regExp.ExpenseName,
                                   Amount = regExp.Amount,
                                   TimeSpan = (TimeOption)regExp.TimeSpan
                               });
            LimitParam limitParams = new LimitParam(_db.Params);
            TempData["TimeSpan"] = timeSpan ?? "total";
            TempData["SortOrder"] = sortOrder ?? "date_desc";
            TimeSpanOption timeSpanOption;

            ViewBag.DateSortParm = sortOrder == "date_desc" ? "date" : "date_desc";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.AmountSortParm = sortOrder == "amount" ? "amount_desc" : "amount";

            string availableContainerClass = "";
            switch (timeSpan)
            {
                case "today":
                    expEntryList = expEntryList?.Where(o => o.Date.Date == DateTime.Today);
                    timeSpanOption = new Today();
                    break;
                case "week":
                    expEntryList = expEntryList?.Where(o => ISOWeek.GetWeekOfYear(o.Date) == ISOWeek.GetWeekOfYear(DateTime.Today));
                    timeSpanOption = new Week();
                    break;
                case "month":
                    expEntryList = expEntryList?.Where(o => o.Date.Month == DateTime.Today.Month);
                    timeSpanOption = new Month();
                    break;
                default:
                    timeSpanOption = new TimeSpanOption();
                    availableContainerClass = "d-none";
                    break;
            }
            timeSpanOption.SetLimit(limitParams);
            regExpList = timeSpanOption.SetRegularExpensesList(regExpList);
            
            decimal total = (expEntryList?.Sum(x => x.Amount) ?? 0) + (regExpList?.Sum(x => x.Amount) ?? 0);
            ViewData["AvailableContainerClass"] = availableContainerClass;
            ViewData["Available"] = (timeSpanOption.Limit - total).ToString("N");
            ViewData["Total"] = (float)total;
            ViewData["TotalString"] = total.ToString("N");
            ViewData["LimitAmount"] = timeSpanOption.Limit;
            ViewData["TimeSpanLabel"] = timeSpanOption.Label;
            switch (sortOrder)
            {
                case "name":
                    expEntryList = expEntryList?.OrderBy(o => o.ExpenseName).ToList();
                    regExpList = regExpList?.OrderBy(o => o.ExpenseName).ToList();
                    break;
                case "name_desc":
                    expEntryList = expEntryList?.OrderByDescending(o => o.ExpenseName).ToList();
                    regExpList = regExpList?.OrderByDescending(o => o.ExpenseName).ToList();
                    break;
                case "amount":
                    expEntryList = expEntryList?.OrderBy(o => o.Amount).ToList();
                    regExpList = regExpList?.OrderBy(o => o.Amount).ToList();
                    break;
                case "amount_desc":
                    expEntryList = expEntryList?.OrderByDescending(o => o.Amount).ToList();
                    regExpList = regExpList?.OrderByDescending(o => o.Amount).ToList();
                    break;
                case "date":
                    expEntryList = expEntryList?.OrderBy(o => o.Date).ToList();
                    regExpList = regExpList?.OrderBy(o => o.TimeSpan).ToList();
                    break;
                default:
                    expEntryList = expEntryList?.OrderByDescending(o => o.Date).ToList();
                    regExpList = regExpList?.OrderByDescending(o => o.TimeSpan).ToList();
                    break;
            }

            return View(new ExpenseManagerVM() { ExpenseEntryVMs = expEntryList, RegularExpenseVMs = regExpList });
        }

        public IActionResult Create()
        {
            return View();
        }
        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExpenseEntryVM expenseEntryVM)
        {
            if (ModelState.IsValid)
            {
                // Add Expense if it doesn't exist already
                var expense = new Expense() { ExpenseName = expenseEntryVM.ExpenseName, Amount = (decimal)expenseEntryVM.Amount };
                Expression<Func<Expense, bool>> predicate = (x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                _db.Expenses.AddIfNotExists<Expense>(expense, predicate);
                _db.SaveChanges();
                // Add ExpenseEntry if it doesn't exist already
                var expenseInDb = _db.Expenses.FirstOrDefault(predicate);
                ExpenseEntry expenseEntry = new ExpenseEntry()
                {
                    ExpenseId = expenseInDb.Id,
                    Expense = expenseInDb,
                    Date = expenseEntryVM.Date
                };

                _db.ExpenseEntries.AddIfNotExists(expenseEntry, x => x.Date == expenseEntry.Date && x.ExpenseId == expenseEntry.ExpenseId);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(expenseEntryVM);
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
            ExpenseEntryVM expenseEntryVM = new ExpenseEntryVM()
            {
                EntryId = expenseEntry.EntryId,
                ExpenseId = expenseEntry.ExpenseId,
                Expense = expense,
                ExpenseName = expense.ExpenseName,
                Amount = expense.Amount,
                Date = expenseEntry.Date
            };
            return View(expenseEntryVM);

        }
        // POST Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ExpenseEntryVM expenseEntryVM)
        {
            if (ModelState.IsValid)
            {
                var expense = new Expense()
                {
                    Id = (int)expenseEntryVM.ExpenseId,
                    ExpenseName = expenseEntryVM.ExpenseName,
                    Amount = (decimal)expenseEntryVM.Amount
                };
                var expenseInDb = _db.Expenses.Find(expense.Id);
                _db.Entry(expenseInDb).State = EntityState.Detached;
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
                        expenseEntryVM.ExpenseId = 0;
                        _db.Expenses.AddIfNotExists<Expense>(expense, x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                    }
                    _db.SaveChanges();
                    // get new ExpenseId if it was modified 
                    if (expenseEntryVM.ExpenseId == 0)
                    {
                        expense = _db.Expenses.FirstOrDefault(x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                        expenseEntryVM.ExpenseId = expense.Id;
                    }
                }
                // update ExpenseEntry entity
                ExpenseEntry expenseEntry = new ExpenseEntry()
                {
                    ExpenseId = expenseEntryVM.EntryId,
                    EntryId = expenseEntryVM.EntryId,
                    Expense = expense,
                    Date = expenseEntryVM.Date
                };
                _db.ExpenseEntries.Update(expenseEntry);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(expenseEntryVM);

        }
        // POST Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string deleteId)
        {
            var expenseEntry = _db.ExpenseEntries.Find(Convert.ToInt32(deleteId));
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
