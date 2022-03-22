using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public class RegExpViewModel
    {
        public int RegularExpenseId { get; set; }
        public int ExpenseId { get; set; }
        public string ExpenseName { get; set; }
        public decimal Amount { get; set; }
        public TimeOption TimeSpan { get; set; }
        public RegExpViewModel(RegularExpense regExpense)
        {
            RegularExpenseId = regExpense.RegularExpenseId;
            ExpenseId = regExpense.ExpenseId;
            ExpenseName = regExpense.Expense.ExpenseName;
            Amount = regExpense.Expense.Amount;
            TimeSpan = (TimeOption)regExpense.TimeSpan;
        }
    }
}
