using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public class RegExpViewModel : RegularExpense
    {
        public string ExpenseName { get; set; }
        public decimal Amount { get; set; }
        public new TimeOption TimeSpan { get; set; }
        public RegExpViewModel() { }
        public RegExpViewModel(RegularExpense regularExpense)
            : base(regularExpense) 
        {
            ExpenseName = Expense.ExpenseName;
            Amount = Expense.Amount;
            TimeSpan = (TimeOption)base.TimeSpan;
        }
    }
}
