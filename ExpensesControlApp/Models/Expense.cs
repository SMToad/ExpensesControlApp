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
        [Required(ErrorMessage = "The name of the expense is required")]
        public string ExpenseName { get; set; }

        [Required(ErrorMessage = "The amount of the expense is required")]
        [Column(TypeName = "smallmoney")]
        [Range(0.01, float.MaxValue, ErrorMessage = "The amount must be greater than zero")]
        public decimal Amount { get; set; }
    }
    
    
}
