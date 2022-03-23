using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public class ExpenseManagerViewModel
    {
        public IEnumerable<ExpenseEntry> ExpenseEntries { get; set; }
        public IEnumerable<RegExpViewModel> RegularExpenses { get; set; }
        public LimitParam LimitParam { get; set; }
    }
}
