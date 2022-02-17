using DimensionService.Common;
using DimensionService.Models.DimensionModels;

namespace DimensionService.Dao.LoginInfo
{
    public interface ILoginInfoDAO
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="useDevice">登录设备</param>
        /// <param name="dateTime">登录时间</param>
        /// <returns></returns>
        bool UserLogin(string userID, ClassHelper.UseDevice useDevice, DateTime dateTime);

        /// <summary>
        /// 有效登录信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="useDevice">登录设备</param>
        /// <returns></returns>
        LoginInfoModel ValidLoginInfo(string userID, ClassHelper.UseDevice useDevice);
    }
}
