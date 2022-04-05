
using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class TimeSpan
    {
        public virtual string Label { get => "Expenses"; }
        public virtual decimal Limit { get; internal set; }
        public virtual void SetLimit(Limit limit)
        {
            Limit = 0;
        }
        public virtual IEnumerable<ExpenseEntryVM> Filter(IEnumerable<ExpenseEntryVM> expEntryList)
        {
            return expEntryList;
        }
        public virtual IEnumerable<RegularExpenseVM> Filter(IEnumerable<RegularExpenseVM> regExpList)
        {
            return null;
        }
        
    }
    
}
