using DimensionClient.Common;
using DimensionClient.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DimensionClient.Context
{
    public class ClientContext : DbContext
    {
        public DbSet<LoginUserModel> LoginUser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Path.Combine(ClassHelper.programPath, "client.db")}");
        }
    }
}
