using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public class ExpenseManagerVM
    {
        public IEnumerable<ExpenseEntryVM> ExpenseEntryVMs { get; set; }
        public IEnumerable<RegularExpenseVM> RegularExpenseVMs { get; set; }
    }
}
