

using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class Today : TimeSpanOption
    {
        public override string Name { get => "Today"; }
        public override string GetLimit(LimitParam limitParam)
        {
            switch (limitParam.TimeSpan)
            {
                case TimeOption.Weekly:
                    return (Convert.ToDouble(limitParam.Amount) / 7).ToString();
                case TimeOption.Monthly:
                    return (Convert.ToDouble(limitParam.Amount) / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)).ToString();
                default:
                    return string.Empty;
            }
        }
    }
}
