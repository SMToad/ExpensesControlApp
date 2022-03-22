using ExpensesControlApp.Data;
using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;
using ExpensesControlApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpensesControlApp.Controllers
{
    public class RegularExpenseController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RegularExpenseController(ApplicationDbContext db)
        {
            _db = db;
        }
        //GET-Index
        public IActionResult Index(string? timeSpan, string? sortOrder)
        {
            // expense list for the ViewModel
            var expenseList = (from exp in _db.Expenses
                               join regExp in _db.RegularExpenses
                               on exp.Id equals regExp.ExpenseId
                               select new
                               {
                                   Id = regExp.RegularExpenseId,
                                   ExpenseId = exp.Id,
                                   Expense = regExp.Expense,
                                   TimeSpan = regExp.TimeSpan
                               }).ToList()
                               .Select(regExp => new RegExpViewModel(new RegularExpense()
                               {
                                   RegularExpenseId = regExp.Id,
                                   ExpenseId = regExp.Id,
                                   Expense = regExp.Expense,
                                   TimeSpan = regExp.TimeSpan
                               }));
            TempData["SortOrder"] = sortOrder ?? "name";

            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.AmountSortParm = sortOrder == "amount" ? "amount_desc" : "amount";
            ViewBag.TimeSortParm = sortOrder == "time_desc" ? "time" : "time_desc";

            switch (sortOrder)
            {
                case "name_desc":
                    expenseList = expenseList.OrderByDescending(o => o.ExpenseName).ToList();
                    break;
                case "amount":
                    expenseList = expenseList.OrderBy(o => o.Amount).ToList();
                    break;
                case "amount_desc":
                    expenseList = expenseList.OrderByDescending(o => o.Amount).ToList();
                    break;
                case "time":
                    expenseList = expenseList.OrderBy(o => o.TimeSpan).ToList();
                    break;
                case "time_desc":
                    expenseList = expenseList.OrderByDescending(o => o.TimeSpan).ToList();
                    break;
                default:
                    expenseList = expenseList.OrderBy(o => o.ExpenseName).ToList();
                    break;
            }

            return View(expenseList);
        }

        public IActionResult Create()
        {
            return View();
        }
        //POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegExpViewModel regExpViewModel)
        {
            // Add Expense if it doesn't exist already
            var expense = new Expense() { ExpenseName = regExpViewModel.ExpenseName, Amount = regExpViewModel.Amount };
            Expression<Func<Expense, bool>> predicate = (x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
            _db.Expenses.AddIfNotExists<Expense>(expense, predicate);
            _db.SaveChanges();
            // Add ExpenseEntry if it doesn't exist already
            var expenseInDb = _db.Expenses.FirstOrDefault(predicate);
            var regExpense = new RegularExpense()
            {
                Expense = expenseInDb,
                ExpenseId = expenseInDb.Id,
                TimeSpan = (int)regExpViewModel.TimeSpan
            };
            //if (ModelState.IsValid)
            //{
            _db.RegularExpenses.AddIfNotExists(regExpense, x => x.TimeSpan == regExpense.TimeSpan && x.ExpenseId == regExpense.ExpenseId);
            _db.SaveChanges();
            return RedirectToAction("Index");
            //}
            //return View(expenseEntry);
        }
        // GET Update
        public IActionResult Update(int? regExpenseId)
        {
            if (regExpenseId == null || regExpenseId == 0)
                return NotFound();
            var regExpense = _db.RegularExpenses.Find(regExpenseId);
            if (regExpense == null)
                return NotFound();
            var expense = _db.Expenses.Find(regExpense.ExpenseId);
            regExpense.Expense = expense;
            return View(new RegExpViewModel(regExpense));

        }
        // POST Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(RegExpViewModel regExpViewModel)
        {
            //if (ModelState.IsValid)
            //{
            var expense = new Expense()
            {
                Id = regExpViewModel.ExpenseId,
                ExpenseName = regExpViewModel.ExpenseName,
                Amount = regExpViewModel.Amount
            };
            var expenseInDb = _db.Expenses.Find(expense.Id);
            // if Expense entity was modified
            if (expenseInDb.ExpenseName != expense.ExpenseName || expenseInDb.Amount != expense.Amount)
            {
                // update if this Expense entity isn't connected to other ExpenseEntry entities
                if (_db.RegularExpenses.Where(o => o.ExpenseId == expense.Id).Count() == 1)
                    _db.Expenses.Update(expense);
                else
                {
                    // else add a new Expense entity if it doesn't already exist
                    expense.Id = 0;
                    _db.Expenses.AddIfNotExists<Expense>(expense, x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                }
                _db.SaveChanges();
                // get new ExpenseId if it was modified 
                if (expense.Id == 0)
                {
                    expense = _db.Expenses.FirstOrDefault(x => x.ExpenseName == expense.ExpenseName && x.Amount == expense.Amount);
                }
            }
            _db.Entry(expenseInDb).State = EntityState.Detached;
            // update ExpenseEntry entity
            var regExpense = new RegularExpense()
            {
                Expense = expense,
                ExpenseId = expense.Id,
                TimeSpan = (int)regExpViewModel.TimeSpan
            };
            _db.RegularExpenses.Update(regExpense);
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
            var regExpense = _db.RegularExpenses.Find(Convert.ToInt32(entryId));
            if (regExpense is null)
            {
                return NotFound();
            }
            _db.RegularExpenses.Remove(regExpense);
            _db.SaveChanges();
            int expenseId = regExpense.ExpenseId;
            if (!_db.RegularExpenses.Any(o => o.ExpenseId == expenseId))
            {
                var expense = _db.Expenses.Find(expenseId);
                _db.Expenses.Remove(expense);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
