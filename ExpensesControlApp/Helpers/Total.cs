
using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class Total : TimeSpanOption
    {
        public override string Name { get => "All Time"; }
        public override string GetLimit(LimitParam limitParam)
        {
            return "0";
        }
    }
}
