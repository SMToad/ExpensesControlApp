

using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class Today : TimeSpanOption
    {
        public override string Label { get => base.Label + " for today"; }
        public override void SetLimit(LimitParam limitParam)
        {
            switch (limitParam.TimeSpan)
            {
                case TimeOption.Weekly:
                    Limit = Convert.ToDecimal(limitParam.Amount) / 7;
                    break;
                case TimeOption.Monthly:
                    Limit = Convert.ToDecimal(limitParam.Amount) / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                    break;
                default:
                    break;
            }
        }
        public override IEnumerable<RegExpViewModel> SetRegularExpensesList(IEnumerable<RegExpViewModel> regExpList)
        {
            var copyList = regExpList.ToList();
            foreach (var regExp in copyList)
                if (regExp.TimeSpan == TimeOption.Weekly)
                    regExp.Amount = regExp.Amount / 7;
                else
                    regExp.Amount = regExp.Amount / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            return copyList;
        }
    }
}
