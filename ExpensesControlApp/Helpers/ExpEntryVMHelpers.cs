using ExpensesControlApp.Models;
using ExpensesControlApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ExpensesControlApp.Helpers
{
    public static class ExpEntryVMHelpers
    {
        public static IEnumerable<ExpenseEntryVM> GetVMList(DbSet<Expense> expDbSet, DbSet<ExpenseEntry> expEntryDbSet)
        {
            return (from exp in expDbSet
                    join expEntry in expEntryDbSet
                    on exp.Id equals expEntry.ExpenseId
                    select new
                    {
                        Id = expEntry.EntryId,
                        ExpenseId = exp.Id,
                        ExpenseName = exp.ExpenseName,
                        Amount = exp.Amount,
                        Date = expEntry.Date

                    }).ToList()
                               .Select(entry => new ExpenseEntryVM()
                               {
                                   EntryId = entry.Id,
                                   ExpenseId = entry.ExpenseId,
                                   ExpenseName = entry.ExpenseName,
                                   Amount = entry.Amount,
                                   Date = entry.Date
                               });
        }
        public static IEnumerable<ExpenseEntryVM> Filter(this IEnumerable<ExpenseEntryVM> expEntryList, TimeSpanView timeSpanView)
        {
            return timeSpanView.Filter(expEntryList);
        }
        public static IEnumerable<ExpenseEntryVM> Sort(this IEnumerable<ExpenseEntryVM> expEntryList, string sortOrder)
        {
            var copyList = expEntryList.ToList();
            switch (sortOrder)
            {
                case "name":
                    copyList = expEntryList.OrderBy(o => o.ExpenseName).ToList();
                    break;
                case "name_desc":
                    copyList = expEntryList.OrderByDescending(o => o.ExpenseName).ToList();
                    break;
                case "amount":
                    copyList = expEntryList.OrderBy(o => o.Amount).ToList();
                    break;
                case "amount_desc":
                    copyList = expEntryList.OrderByDescending(o => o.Amount).ToList();
                    break;
                case "time":
                    copyList = expEntryList.OrderBy(o => o.Date).ToList();
                    break;
                default:
                    copyList = expEntryList.OrderByDescending(o => o.Date).ToList();
                    break;
            }
            return copyList;
        }

    }
}
