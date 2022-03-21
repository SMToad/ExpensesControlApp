using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public class ExpenseEntryViewModel
    {
        public IEnumerable<ExpenseEntry> ExpenseEntries { get; set; }
        public LimitParam LimitParam { get; set; }
    }
}
