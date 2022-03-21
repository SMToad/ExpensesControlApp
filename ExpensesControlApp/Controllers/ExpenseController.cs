using ExpensesControlApp.Data;
using ExpensesControlApp.Models;
using ExpensesControlApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ExpensesControlApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ExpenseController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                Expression<Func<Expense, bool>> predicate = (x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                _db.Expenses.AddIfNotExists<Expense>(expense, predicate);
                _db.SaveChanges();
                var expenseInDb = _db.Expenses.FirstOrDefault(predicate);
                
                ExpenseEntry expenseEntry = new ExpenseEntry()
                {
                    ExpenseId = expenseInDb.Id
                };
                return RedirectToAction("Create", "ExpenseEntry", expenseEntry);
            }
            return View(expense);
        }
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var expense = _db.Expenses.Find(id);
            if (expense == null)
                return NotFound();
            return View(expense);
        }

        // POST Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Expense expense)
        {
            //server side validation error handling
            if (ModelState.IsValid)
            {
                if (!_db.ExpenseEntries.Any(o => o.ExpenseId == expense.Id))
                    _db.Expenses.Update(expense);
                else
                {
                    expense.Id = 0;
                    _db.Expenses.AddIfNotExists<Expense>(expense, x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                }
                _db.SaveChanges();
                expense = _db.Expenses.FirstOrDefault(x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                ExpenseEntry expenseEntry = new ExpenseEntry()
                {
                    Expense = expense,
                    ExpenseId = expense.Id
                };
                return RedirectToAction("Create", "ExpenseEntry", expenseEntry);
            }
            return View(expense);
        }
        // POST Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Expense expense)
        {
            if (!_db.ExpenseEntries.Any(o => o.ExpenseId == expense.Id))
            {
                _db.Expenses.Remove(expense);
                _db.SaveChanges();
            }
            return RedirectToAction("Index", "ExpenseEntry");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEntry(ExpenseEntry expenseEntry)
        {
            var expense = _db.Expenses.Find(expenseEntry.ExpenseId);
            if (!_db.ExpenseEntries.Any(o => o.ExpenseId == expense.Id))
            {
                _db.Expenses.Remove(expense);
                _db.SaveChanges();
            }
            return RedirectToAction("Index", "ExpenseEntry");
        }
    }
}
