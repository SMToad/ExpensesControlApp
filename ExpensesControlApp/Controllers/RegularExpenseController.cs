using ExpensesControlApp.Data;
using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;
using ExpensesControlApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index(string? sortOrder)
        {
            // expense list for the ViewModel
            var expenseList = (from exp in _db.Expenses
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
            TempData["SortOrder"] = sortOrder ?? "name";

            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.AmountSortParm = sortOrder == "amount" ? "amount_desc" : "amount";
            ViewBag.TimeSortParm = sortOrder == "time" ? "time_desc" : "time";

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
        public IActionResult Create(RegularExpenseVM regularExpenseVM)
        {
            if (ModelState.IsValid)
            {
            Expression<Func<Expense, bool>> predicate = (x => x.ExpenseName == regularExpenseVM.ExpenseName && x.Amount == regularExpenseVM.Amount);

            var expenseInDb = _db.Expenses.FirstOrDefault(predicate);
            if (expenseInDb == null)
            { // Add Expense if it doesn't exist already
                var expense = new Expense() { ExpenseName = regularExpenseVM.ExpenseName, Amount = (decimal)regularExpenseVM.Amount };
                 _db.Expenses.AddIfNotExists<Expense>(expense, predicate);
                _db.SaveChanges();
                // Add ExpenseEntry if it doesn't exist already
                expenseInDb = _db.Expenses.FirstOrDefault(predicate);
                var regExpense = new RegularExpense()
                {
                    Expense = expenseInDb,
                    ExpenseId = expenseInDb.Id,
                    TimeSpan = (int)regularExpenseVM.TimeSpan
                };
                
                _db.RegularExpenses.AddIfNotExists(regExpense, x => x.TimeSpan == regExpense.TimeSpan && x.ExpenseId == regExpense.ExpenseId);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            }
            return View(regularExpenseVM);
        }
        // GET Update
        public IActionResult Update(int? regExpenseId)
        {
            if (regExpenseId == null || regExpenseId == 0)
                return NotFound();
            var regularExpense = _db.RegularExpenses.Find(regExpenseId);
            if (regularExpense == null)
                return NotFound();
            var expense = _db.Expenses.Find(regularExpense.ExpenseId);
            RegularExpenseVM regularExpenseVM = new RegularExpenseVM()
            {
                RegularExpenseId = regularExpense.RegularExpenseId,
                ExpenseId = regularExpense.ExpenseId,
                Expense = expense,
                ExpenseName = expense.ExpenseName,
                Amount = expense.Amount,
                TimeSpan = (TimeOption)regularExpense.TimeSpan
            };
            return View(regularExpenseVM);

        }
        // POST Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(RegularExpenseVM regularExpenseVM)
        {
            if (ModelState.IsValid)
            {
                var expense = new Expense()
                {
                    Id = (int)regularExpenseVM.ExpenseId,
                    ExpenseName = regularExpenseVM.ExpenseName,
                    Amount = (decimal)regularExpenseVM.Amount
                };
                _db.Expenses.Update(expense);
                // update ExpenseEntry entity
                var regularExpense = new RegularExpense()
                {
                    RegularExpenseId = regularExpenseVM.RegularExpenseId,
                    Expense = expense,
                    ExpenseId = expense.Id,
                    TimeSpan = (int)regularExpenseVM.TimeSpan
                };
                _db.RegularExpenses.Update(regularExpense);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(regularExpenseVM);
        }
        // POST Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string deleteId)
        {
            var regularExpense = _db.RegularExpenses.Find(Convert.ToInt32(deleteId));
            if (regularExpense is null)
            {
                return NotFound();
            }
            _db.RegularExpenses.Remove(regularExpense);
            var expense = _db.Expenses.Find(regularExpense.ExpenseId);
            _db.Expenses.Remove(expense);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
