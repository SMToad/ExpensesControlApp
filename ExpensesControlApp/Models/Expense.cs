using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesControlApp.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }
        public string ExpenseName { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal Amount { get; set; }
    }
    
    
}
