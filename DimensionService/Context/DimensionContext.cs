using DimensionService.Models.DimensionModels;
using Microsoft.EntityFrameworkCore;

namespace DimensionService.Context
{
    public class DimensionContext : DbContext
    {
        public DbSet<UserInfoModel> UserInfo { get; set; }
        public DbSet<LoginInfoModel> LoginInfo { get; set; }
        public DbSet<FriendInfoModel> FriendInfo { get; set; }
        public DbSet<ChatLinkModel> ChatLink { get; set; }
        public DbSet<ChatColumnModel> ChatColumn { get; set; }
        public DbSet<ChatMessagesModel> ChatMessages { get; set; }
        public DbSet<CallRoomModel> CallRoom { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 阿里云 47.96.133.119 密码 Wangxi55
            optionsBuilder.UseSqlServer("Data Source=47.96.133.119;Initial Catalog=Dimension;Persist Security Info=True;User ID=sa;Password=Wangxi55");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
