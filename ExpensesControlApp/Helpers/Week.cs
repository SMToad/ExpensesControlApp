
using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class Week : TimeSpanOption
    {
        public override string Name { get => "This Week"; }
        public override string GetLimit(LimitParam limitParam)
        {
            switch (limitParam.TimeSpan)
            {
                case TimeOptions.Weekly:
                    return limitParam.Amount.ToString();
                case TimeOptions.Monthly:
                    return (Convert.ToDouble(limitParam.Amount) / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) * 7).ToString();
                default:
                    return string.Empty;
            }
        }
    }
}
