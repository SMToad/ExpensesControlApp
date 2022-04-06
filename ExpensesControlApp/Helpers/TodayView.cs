

using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class TodayView : TimeSpanView
    {
        public override string Label { get => base.Label + " for today"; }
        public override void SetLimit(Limit limit)
        {
            switch (limit.TimeSpan)
            {
                case TimeSpanOption.Weekly:
                    Limit = Convert.ToDecimal(limit.Amount) / 7;
                    break;
                case TimeSpanOption.Monthly:
                    Limit = Convert.ToDecimal(limit.Amount) / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                    break;
                default:
                    break;
            }
        }
        public override IEnumerable<ExpenseEntryVM> Filter(IEnumerable<ExpenseEntryVM> expEntryList)
        {
            return expEntryList.Where(o => o.Date.Date == DateTime.Today);
        }
        public override IEnumerable<RegularExpenseVM> Filter(IEnumerable<RegularExpenseVM> regExpList)
        {
            var copyList = regExpList.ToList();
            foreach (var regExp in copyList)
                if (regExp.TimeSpan == TimeSpanOption.Weekly)
                    regExp.Amount = regExp.Amount / 7;
                else
                    regExp.Amount = regExp.Amount / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            return copyList;
        }
    }
}
