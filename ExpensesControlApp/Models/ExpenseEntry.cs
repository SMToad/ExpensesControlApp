using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesControlApp.Models
{
    //[Bind(Exclude="ExpenseId")]
    public class ExpenseEntry
    {
        [Key]
        public int EntryId { get; set; }

        [ForeignKey("Expense")]
        
        public int ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}
