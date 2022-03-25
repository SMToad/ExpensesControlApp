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
        [Required]
        public int TimeSpan { get; set; }
        public RegularExpense() { }
        protected RegularExpense(RegularExpense regularExpense)
        {
            RegularExpenseId = regularExpense.RegularExpenseId;
            ExpenseId = regularExpense.ExpenseId;
            Expense = regularExpense.Expense;
            TimeSpan = regularExpense.TimeSpan;
        }
    }
}
