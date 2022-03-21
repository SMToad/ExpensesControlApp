using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public enum TimeOptions
    {
        Weekly,
        Monthly
    }
    public class LimitParam
    {
        public int Amount { get; set; }
        public TimeOptions TimeSpan { get; set; }
        public LimitParam() { }
        public LimitParam(IEnumerable<Param> pars)
        {
            Amount = pars.Where(p => p.Key == "limitAmount").Select(p => Convert.ToInt32(p.Value)).First<int>();
            TimeSpan =(TimeOptions)pars.Where(p => p.Key == "limitTimeSpan").Select(p => Convert.ToInt32(p.Value)).First<int>();
        }
    }
}
