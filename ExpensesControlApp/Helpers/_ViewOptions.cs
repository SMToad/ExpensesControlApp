namespace ExpensesControlApp.Helpers
{
    public class _ViewOptions
    { 
        public string TimeSpan { get; set; }
        public string SortOrder { get; set; }
        public _ViewOptions(string timeSpan, string sortOrder)
        {
            TimeSpan = timeSpan;
            SortOrder = sortOrder;
        }
    }
}
