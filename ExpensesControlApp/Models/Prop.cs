using System.ComponentModel.DataAnnotations;

namespace ExpensesControlApp.Models
{
    public class Prop
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
