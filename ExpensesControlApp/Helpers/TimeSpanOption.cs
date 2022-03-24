
using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class TimeSpanOption
    {
        public virtual string Name { get => ""; }
        public virtual decimal Limit { get; internal set; }
        public virtual void SetLimit(LimitParam limitParam)
        {
            Limit = 0;
        }
        public virtual IEnumerable<RegExpViewModel> SetRegularExpensesList(IEnumerable<RegExpViewModel> regExpList)
        {
            return null;
        }
    }
    
}
