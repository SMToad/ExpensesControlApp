using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class Month : TimeSpanOption
    {
        public override string Name { get => "This Month"; }
        public override string GetLimit(LimitParam limitParam)
        {
            switch (limitParam.TimeSpan)
            {
                case TimeOption.Weekly:
                    return (Convert.ToDouble(limitParam.Amount) / 7 * DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)).ToString();
                case TimeOption.Monthly:
                    return limitParam.Amount.ToString();
                default:
                    return string.Empty;
            }
        }
    }
}
