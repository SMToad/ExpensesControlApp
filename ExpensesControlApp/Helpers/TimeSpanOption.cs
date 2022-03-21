
using ExpensesControlApp.ViewModels;

namespace ExpensesControlApp.Helpers
{
    public class TimeSpanOption
    {
        public virtual string Name { get; }
        public virtual string GetLimit(LimitParam limitParam) { return string.Empty; }
    }
    
}
