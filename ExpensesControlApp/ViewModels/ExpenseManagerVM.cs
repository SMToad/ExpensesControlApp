using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;

namespace ExpensesControlApp.ViewModels
{
    public class ExpenseManagerVM
    {
        public IEnumerable<ExpenseEntryVM> ExpenseEntryVMs { get; set; }
        public IEnumerable<RegularExpenseVM> RegularExpenseVMs { get; set; }
        public string Title { get; set; }
        public decimal Limit { get; set; }
        public Dictionary<string, string> Available { get; set; }
        public decimal Total { get; set; }

        public void ApplyTimeSpan(string timeInput, Limit limit)
        {
            TimeSpanView timeSpanView;
            Available = new Dictionary<string, string>();
            Available["ContainerClass"] = "";
            switch (timeInput)
            {
                case "today":
                    timeSpanView = new TodayView();
                    break;
                case "week":
                    timeSpanView = new WeekView();
                    break;
                case "month":
                    timeSpanView = new MonthView();
                    break;
                default:
                    timeSpanView = new TimeSpanView();
                    Available["ContainerClass"] = "d-none ";
                    break;
            }
            timeSpanView.SetLimit(limit);
            ExpenseEntryVMs = ExpenseEntryVMs.Filter(timeSpanView);
            RegularExpenseVMs = RegularExpenseVMs.Filter(timeSpanView);

            Title = timeSpanView.Label;
            Limit = timeSpanView.Limit;
            Total = (ExpenseEntryVMs?.Sum(x => x.Amount) ?? 0) + (RegularExpenseVMs?.Sum(x => x.Amount) ?? 0);
            decimal available = Limit - Total;
            if (available < 0)
            {
                available *= -1;
                Available["ContainerClass"] += "text-danger";
                Available["Label"] = "Overlimit:";
            }
            else
            {
                Available["ContainerClass"] += "text-success";
                Available["Label"] = "Available:";
            }
            Available["Value"] = available.ToString("N");
        }
    }
}
