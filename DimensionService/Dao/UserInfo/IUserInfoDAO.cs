using DimensionService.Models.DimensionModels;
using System.Collections.Generic;

namespace DimensionService.Dao.UserInfo
{
    public interface IUserInfoDAO
    {
        /// <summary>
        /// 用户ID查询用户信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="verify">验证</param>
        /// <returns></returns>
        UserInfoModel UserInfoFindForUserID(string userID, bool verify = false);

        /// <summary>
        /// 昵称查询用户信息
        /// </summary>
        /// <param name="nickName">昵称</param>
        /// <param name="verify">验证</param>
        /// <returns></returns>
        UserInfoModel UserInfoFindForNickName(string nickName, bool verify = false);

        /// <summary>
        /// 手机号查询用户信息
        /// </summary>
        /// <param name="phoneNumber">手机号</param>
        /// <param name="verify">验证</param>
        /// <returns></returns>
        UserInfoModel UserInfoFindForPhoneNumber(string phoneNumber, bool verify = false);

        /// <summary>
        /// 邮箱查询用户信息
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="verify">验证</param>
        /// <returns></returns>
        UserInfoModel UserInfoFindForEmail(string email, bool verify = false);

        /// <summary>
        /// 查询手机号是否存在
        /// </summary>
        /// <param name="phoneNumber">手机号</param>
        /// <returns>true false</returns>
        bool PhoneNumberExist(string phoneNumber);

        /// <summary>
        /// 查询邮箱是否存在
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>true false</returns>
        bool EmailExist(string email);

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        bool UserInfoAdd(UserInfoModel userInfo);

        /// <summary>
        /// 更新在线状态
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="onLine">在线状态</param>
        /// <returns></returns>
        bool UpdatedOnLine(string userID, bool onLine);

        /// <summary>
        /// 获取用户信息集合
        /// </summary>
        /// <param name="userIDs">用户ID集合</param>
        /// <returns></returns>
        List<UserInfoModel> GetUserInfos(List<string> userIDs);
    }
}
