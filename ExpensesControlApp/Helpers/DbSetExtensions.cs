using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpensesControlApp.Helpers
{
    public static class DbSetExtensions
    {
        public static void AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate) 
            where T : class, new()
        {
            var exists = dbSet.Any(predicate);
            if (!exists) 
                dbSet.Add(entity);
        }
    }

}
