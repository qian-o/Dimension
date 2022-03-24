using DimensionService.Common;
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
            optionsBuilder.UseSqlServer(ClassHelper.connection);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
