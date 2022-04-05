using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class Month : TimeSpan
    {
        public override string Label { get => base.Label + " for this Month"; }
        public override void SetLimit(Limit limit)
        {
            switch (limit.TimeSpan)
            {
                case TimeSpanOption.Weekly:
                    Limit = Convert.ToDecimal(limit.Amount) / 7 * DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                    break;
                case TimeSpanOption.Monthly:
                    Limit = (decimal)limit.Amount;
                    break;
                default:
                    break;
            }
        }
        public override IEnumerable<ExpenseEntryVM> Filter(IEnumerable<ExpenseEntryVM> expEntryList)
        {
            return expEntryList.Where(o => o.Date.Month == DateTime.Today.Month);
        }
        public override IEnumerable<RegularExpenseVM> Filter(IEnumerable<RegularExpenseVM> regExpList)
        {
            var copyList = regExpList.ToList();
            foreach (var regExp in copyList.Where(x => x.TimeSpan == TimeSpanOption.Weekly))
                regExp.Amount = regExp.Amount / 7 * DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            return copyList;
        }
    }
}
