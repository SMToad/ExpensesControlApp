
using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class TimeSpanOption
    {
        public virtual string Label { get => "Expenses"; }
        public virtual decimal Limit { get; internal set; }
        public virtual void SetLimit(LimitParam limitParam)
        {
            Limit = 0;
        }
        public virtual IEnumerable<RegularExpenseVM> SetRegularExpensesList(IEnumerable<RegularExpenseVM> regExpList)
        {
            return null;
        }
        
    }
    
}
