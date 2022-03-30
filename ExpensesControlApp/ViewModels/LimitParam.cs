using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesControlApp.ViewModels
{
    public class LimitParam
    {
        [Required(ErrorMessage = "The amount of the expense is required")]
        [Column(TypeName = "smallmoney")]
        [Range(0.01, float.MaxValue, ErrorMessage = "The amount must be greater than zero")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Time span of the expense is required")]
        public TimeOption TimeSpan { get; set; }
        public LimitParam() { }
        public LimitParam(IEnumerable<Param> pars)
        {
            Amount = pars.Where(p => p.Key == "limitAmount").Select(p => Convert.ToDecimal(p.Value)).First<decimal>();
            TimeSpan =(TimeOption)pars.Where(p => p.Key == "limitTimeSpan").Select(p => Convert.ToInt32(p.Value)).First<int>();
        }
    }
}
