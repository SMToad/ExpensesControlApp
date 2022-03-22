using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public class LimitParam
    {
        public int Amount { get; set; }
        public TimeOption TimeSpan { get; set; }
        public LimitParam() { }
        public LimitParam(IEnumerable<Param> pars)
        {
            Amount = pars.Where(p => p.Key == "limitAmount").Select(p => Convert.ToInt32(p.Value)).First<int>();
            TimeSpan =(TimeOption)pars.Where(p => p.Key == "limitTimeSpan").Select(p => Convert.ToInt32(p.Value)).First<int>();
        }
    }
}
