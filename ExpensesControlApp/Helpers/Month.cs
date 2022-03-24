using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class Month : TimeSpanOption
    {
        public override string Name { get => "This Month"; }
        public override void SetLimit(LimitParam limitParam)
        {
            switch (limitParam.TimeSpan)
            {
                case TimeOption.Weekly:
                    Limit = Convert.ToDecimal(limitParam.Amount) / 7 * DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                    break;
                case TimeOption.Monthly:
                    Limit = limitParam.Amount;
                    break;
                default:
                    break;
            }
        }
        public override IEnumerable<RegExpViewModel> SetRegularExpensesList(IEnumerable<RegExpViewModel> regExpList)
        {
            var copyList = regExpList.ToList();
            foreach (var regExp in copyList.Where(x => x.TimeSpan == TimeOption.Weekly))
                regExp.Amount = regExp.Amount / 7 * DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            return copyList;
        }
    }
}
