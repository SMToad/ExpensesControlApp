using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesControlApp.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Expense Name")]
        [Required(ErrorMessage = "Enter the name of the expense")]
        public string ExpenseName { get; set; }

        [Required(ErrorMessage = "Enter the amount of the expense")]
        [Column(TypeName = "smallmoney")]
        //[Range(0.01, float.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
       // public ICollection<ExpenseEntry> ExpenseDates { get; set; }
    }
    
    
}
