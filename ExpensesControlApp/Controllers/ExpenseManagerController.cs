using ExpensesControlApp.Data;
using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;
using ExpensesControlApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        
        public IActionResult Index(string? timeInput, string? sortInput)
        {
            var expEntryList = ExpEntryVMHelpers.GetVMList(_db.Expenses, _db.ExpenseEntries);
            var regExpList = RegExpVMHelpers.GetVMList(_db.Expenses, _db.RegularExpenses);
            Limit limit = new Limit(_db.Props);
            
            TempData["TimeSpanInput"] = timeInput ?? "total";
            TempData["SortOrderInput"] = sortInput ?? "date_desc";

            ViewBag.DateSortParm = sortInput == "date_desc" ? "date" : "date_desc";
            ViewBag.NameSortParm = sortInput == "name" ? "name_desc" : "name";
            ViewBag.AmountSortParm = sortInput == "amount" ? "amount_desc" : "amount";

            
            expEntryList = expEntryList?.Sort(sortInput);
            regExpList = regExpList?.Sort(sortInput);

            ExpenseManagerVM expenseManagerVM = new ExpenseManagerVM()
            {
                ExpenseEntryVMs = expEntryList,
                RegularExpenseVMs = regExpList
            };
            expenseManagerVM.ApplyTimeSpan(timeInput, limit);

            return View(expenseManagerVM);
        }

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExpenseEntryVM expenseEntryVM)
        {
            if (ModelState.IsValid)
            {
                var expense = new Expense() 
                { 
                    ExpenseName = expenseEntryVM.ExpenseName, 
                    Amount = (decimal)expenseEntryVM.Amount 
                };
                Expression<Func<Expense, bool>> predicate = (
                    x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                _db.Expenses.AddIfNotExists<Expense>(expense, predicate);
                _db.SaveChanges();
                
                var expenseInDb = _db.Expenses.FirstOrDefault(predicate);
                ExpenseEntry expenseEntry = new ExpenseEntry()
                {
                    ExpenseId = expenseInDb.Id,
                    Expense = expenseInDb,
                    Date = expenseEntryVM.Date
                };
                _db.ExpenseEntries.AddIfNotExists(expenseEntry, 
                                                  x => x.Date == expenseEntry.Date && x.ExpenseId == expenseEntry.ExpenseId);
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
                Expression<Func<Expense, bool>> predicate = (
                   x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                _db.Entry(expenseInDb).State = EntityState.Detached;
                
                if (expenseInDb.ExpenseName != expense.ExpenseName || expenseInDb.Amount != expense.Amount)
                {
                    if (_db.ExpenseEntries.Where(o => o.ExpenseId == expense.Id).Count() == 1)
                    {
                        _db.Expenses.Update(expense);
                        _db.SaveChanges();
                    }
                    else
                    {
                        expense.Id = 0;
                        expenseEntryVM.ExpenseId = 0;
                        _db.Expenses.AddIfNotExists<Expense>(expense, predicate);
                        _db.SaveChanges();
                        expense = _db.Expenses.FirstOrDefault(predicate);
                        expenseEntryVM.ExpenseId = expense.Id;
                    }
                }

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
