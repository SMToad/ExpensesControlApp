using DataAnnotationsExtensions;
using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ExpensesControlApp.ViewModels
{
    public class Limit
    {
        [Required(ErrorMessage = "Limit Amount is required")]
        [RegularExpression(@"^[0-9]+(,[0-9]{1,3})?$", ErrorMessage = "The Amount has too many digits after the \",\"")]
        [Min(0.01, ErrorMessage = "The Amount should be greater than zero")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "Time span of the limit is required")]
        public TimeSpanOption TimeSpan { get; set; }

        public Limit() { }
        public Limit(IEnumerable<Prop> props)
        {
            Amount = props.Where(p => p.Key == "limitAmount").Select(p => Convert.ToDecimal(p.Value)).First<decimal>();
            TimeSpan =(TimeSpanOption)props.Where(p => p.Key == "limitTimeSpan").Select(p => Convert.ToInt32(p.Value)).First<int>();
        }
    }
}
