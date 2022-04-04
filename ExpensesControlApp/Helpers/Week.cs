
using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class Week : TimeSpanOption
    {
        public override string Label { get => base.Label + " for this Week"; }
        public override void SetLimit(LimitParam limitParam)
        {
            switch (limitParam.TimeSpan)
            {
                case TimeOption.Weekly:
                    Limit = (decimal)limitParam.Amount;
                    break;
                case TimeOption.Monthly:
                    Limit = Convert.ToDecimal(limitParam.Amount) / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) * 7;
                    break;
                default:
                    break;
            }
        }
        public override IEnumerable<RegularExpenseVM> SetRegularExpensesList(IEnumerable<RegularExpenseVM> regExpList)
        {
            var copyList = regExpList.ToList();
            foreach (var regExp in copyList.Where(x => x.TimeSpan == TimeOption.Monthly))
                regExp.Amount = regExp.Amount / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) * 7;
            return copyList;
        }
    }
}
