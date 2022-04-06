
using ExpensesControlApp.ViewModels;
using System.Globalization;

namespace ExpensesControlApp.Helpers
{
    public class WeekView : TimeSpanView
    {
        public override string Label { get => base.Label + " for this Week"; }
        public override void SetLimit(Limit limit)
        {
            switch (limit.TimeSpan)
            {
                case TimeSpanOption.Weekly:
                    Limit = (decimal)limit.Amount;
                    break;
                case TimeSpanOption.Monthly:
                    Limit = Convert.ToDecimal(limit.Amount) / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) * 7;
                    break;
                default:
                    break;
            }
        }
        public override IEnumerable<ExpenseEntryVM> Filter(IEnumerable<ExpenseEntryVM> expEntryList)
        {
            return expEntryList.Where(o => ISOWeek.GetWeekOfYear(o.Date) == ISOWeek.GetWeekOfYear(DateTime.Today));
        }
        public override IEnumerable<RegularExpenseVM> Filter(IEnumerable<RegularExpenseVM> regExpList)
        {
            var copyList = regExpList.ToList();
            foreach (var regExp in copyList.Where(x => x.TimeSpan == TimeSpanOption.Monthly))
                regExp.Amount = regExp.Amount / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) * 7;
            return copyList;
        }
    }
}
