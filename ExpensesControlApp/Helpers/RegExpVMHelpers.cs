using ExpensesControlApp.Models;
using ExpensesControlApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ExpensesControlApp.Helpers
{
    public static class RegExpVMHelpers
    {
        public static IEnumerable<RegularExpenseVM> GetVMList(DbSet<Expense> expDbSet, DbSet<RegularExpense> regExpDbSet)
        {
            return (from exp in expDbSet
                    join regExp in regExpDbSet
                    on exp.Id equals regExp.ExpenseId
                    select new
                    {
                        Id = regExp.RegularExpenseId,
                        ExpenseId = exp.Id,
                        ExpenseName = exp.ExpenseName,
                        Amount = exp.Amount,
                        TimeSpan = regExp.TimeSpan

                    }).ToList()
                               .Select(regExp => new RegularExpenseVM()
                               {
                                   RegularExpenseId = regExp.Id,
                                   ExpenseId = regExp.ExpenseId,
                                   ExpenseName = regExp.ExpenseName,
                                   Amount = regExp.Amount,
                                   TimeSpan = (TimeSpanOption)regExp.TimeSpan
                               });
        }
        public static IEnumerable<RegularExpenseVM> Filter(this IEnumerable<RegularExpenseVM> regExpList, TimeSpan timeSpan)
        {
            return timeSpan.Filter(regExpList);
        }
        public static IEnumerable<RegularExpenseVM> Sort(this IEnumerable<RegularExpenseVM> regExpList, string sortOrder)
        {
            var copyList = regExpList.ToList();
            switch (sortOrder)
            {
                case "name":
                    copyList = regExpList.OrderBy(o => o.ExpenseName).ToList();
                    break;
                case "name_desc":
                    copyList = regExpList.OrderByDescending(o => o.ExpenseName).ToList();
                    break;
                case "amount":
                    copyList = regExpList.OrderBy(o => o.Amount).ToList();
                    break;
                case "amount_desc":
                    copyList = regExpList.OrderByDescending(o => o.Amount).ToList();
                    break;
                case "time":
                    copyList = regExpList.OrderBy(o => o.TimeSpan).ToList();
                    break;
                default:
                    copyList = regExpList.OrderByDescending(o => o.TimeSpan).ToList();
                    break;
            }
            return copyList;
        }
    }
}
