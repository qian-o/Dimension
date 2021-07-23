using DimensionService.Models.DimensionModels.FriendInfoModels;
using System.Collections.Generic;

namespace DimensionService.Dao.FriendInfo
{
    public interface IFriendInfoDAO
    {
        /// <summary>
        /// 确认好友关系
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="friendID">好友ID</param>
        /// <returns></returns>
        bool ConfirmFriend(string userID, string friendID);

        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        List<FriendModel> GetFriends(string userID);

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="friendID">好友ID</param>
        /// <param name="verifyInfo">验证信息</param>
        /// <returns></returns>
        bool AddFriend(string userID, string friendID, string verifyInfo);

        /// <summary>
        /// 验证好友
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="friendID">好友ID</param>
        /// <param name="passed">是否通过</param>
        /// <returns></returns>
        bool VerifyFriend(string userID, string friendID, bool passed);

        /// <summary>
        /// 获取新好友列表
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        List<NewFriendModel> GetNewFriends(string userID);

        /// <summary>
        /// 更新好友备注信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="friendID">好友ID</param>
        /// <param name="remarkName">备注名</param>
        /// <param name="remarkInformation">备注信息</param>
        /// <returns></returns>
        bool UpdateRemark(string userID, string friendID, string remarkName, string remarkInformation);
    }
}
