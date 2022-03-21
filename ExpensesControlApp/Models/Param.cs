using System.ComponentModel.DataAnnotations;

namespace ExpensesControlApp.Models
{
    public class Param
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
