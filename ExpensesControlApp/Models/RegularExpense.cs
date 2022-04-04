using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesControlApp.Models
{
    public class RegularExpense
    {
        [Key]
        public int RegularExpenseId { get; set; }
        [ForeignKey("Expense")]
        public int ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }
        public int TimeSpan { get; set; }
    }
}
