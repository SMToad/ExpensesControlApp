using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesControlApp.ViewModels
{
    public class RegExpViewModel : RegularExpense
    {
        [Required(ErrorMessage = "The name of the expense is required")]
        public string ExpenseName { get; set; }
        [Required(ErrorMessage = "The amount of the expense is required")]
        [Column(TypeName = "smallmoney")]
        [Range(0.01, float.MaxValue, ErrorMessage = "The amount must be greater than zero")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Time span of the expense is required")]
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
