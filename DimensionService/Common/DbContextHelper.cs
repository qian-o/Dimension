using Microsoft.EntityFrameworkCore;

namespace DimensionService.Common
{
    public static class DbContextHelper
    {
        public static DbSet<T> GetTable<T>(this DbContext context, string name) where T : class
        {
            return context.GetType().GetProperty(name).GetValue(context) as DbSet<T>;
        }
    }
}
