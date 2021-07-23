using DimensionClient.Context;
using DimensionClient.Models;
using System.Collections.Generic;
using System.Linq;

namespace DimensionClient.Dao.LoginUser
{
    public static class LoginUserDAO
    {
        public static List<LoginUserModel> GetLoginUsers()
        {
            using ClientContext context = new();
            return context.LoginUser.ToList();
        }

        public static bool UpdateLoginUser(LoginUserModel loginUser)
        {
            using ClientContext context = new();
            if (context.LoginUser.Where(item => item.UserID == loginUser.UserID).FirstOrDefault() is LoginUserModel login)
            {
                login.NickName = loginUser.NickName;
                login.LoginName = loginUser.LoginName;
                login.HeadPortrait = loginUser.HeadPortrait;
                if (loginUser.Password != null)
                {
                    login.Password = loginUser.Password;
                }
            }
            else
            {
                context.LoginUser.Add(loginUser);
            }
            return context.SaveChanges() > 0;
        }

        public static bool DeleteLoginUser(int id)
        {
            using ClientContext context = new();
            if (context.LoginUser.Where(item => item.ID == id).FirstOrDefault() is LoginUserModel login)
            {
                context.LoginUser.Remove(login);
            }
            return context.SaveChanges() > 0;
        }
    }
}
