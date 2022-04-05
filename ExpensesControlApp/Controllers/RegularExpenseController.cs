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
       
        public IActionResult Index(string? sortInput)
        {
            var regExpList = RegExpVMHelpers.GetVMList(_db.Expenses, _db.RegularExpenses);

            TempData["SortOrder"] = sortInput ?? "name";

            ViewBag.NameSortParm = sortInput == "name" ? "name_desc" : "name";
            ViewBag.AmountSortParm = sortInput == "amount" ? "amount_desc" : "amount";
            ViewBag.TimeSortParm = sortInput == "time" ? "time_desc" : "time";

            regExpList = regExpList?.Sort(sortInput);

            return View(regExpList);
        }

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegularExpenseVM regularExpenseVM)
        {
            if (ModelState.IsValid)
            {
                Expression<Func<Expense, bool>> predicate = (
                    x => x.ExpenseName == regularExpenseVM.ExpenseName && x.Amount == regularExpenseVM.Amount);

                var expenseInDb = _db.Expenses.FirstOrDefault(predicate);
                if (expenseInDb == null)
                { 
                    var expense = new Expense() 
                    { 
                        ExpenseName = regularExpenseVM.ExpenseName, 
                        Amount = (decimal)regularExpenseVM.Amount 
                    };
                    _db.Expenses.AddIfNotExists<Expense>(expense, predicate);
                    _db.SaveChanges();
                   
                    expenseInDb = _db.Expenses.FirstOrDefault(predicate);
                    var regExpense = new RegularExpense()
                    {
                        Expense = expenseInDb,
                        ExpenseId = expenseInDb.Id,
                        TimeSpan = (int)regularExpenseVM.TimeSpan
                    };

                    _db.RegularExpenses.AddIfNotExists(
                        regExpense, x => x.TimeSpan == regExpense.TimeSpan && x.ExpenseId == regExpense.ExpenseId);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(regularExpenseVM);
        }
        
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
                TimeSpan = (TimeSpanOption)regularExpense.TimeSpan
            };
            return View(regularExpenseVM);

        }
       
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
