using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public class ExpenseManagerViewModel
    {
        public IEnumerable<ExpenseEntry> ExpenseEntries { get; set; }
        public IEnumerable<RegExpViewModel> RegularExpenses { get; set; }
        public TimeSpanOption TimeSpanOption { get; set; }
    }
}
