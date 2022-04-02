using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesControlApp.Models
{
    [Bind(Exclude="ExpenseId")]
    public class ExpenseEntry
    {
        [Key]
        public int EntryId { get; set; }

        [ForeignKey("Expense")]
        
        public int ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }
        [Required(ErrorMessage = "The date of the expense is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "The name of the expense is required")]
        public string ExpenseName { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "The amount of the expense is required")]
        [Column(TypeName = "smallmoney")]
        [Range(0.01, float.MaxValue, ErrorMessage = "The amount must be greater than zero")]
        public decimal Amount { get; set; }
    }
}
