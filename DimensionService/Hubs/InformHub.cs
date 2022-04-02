using Dimension.Domain;
using DimensionService.Common;
using DimensionService.Dao.FriendInfo;
using DimensionService.Dao.UserInfo;
using DimensionService.Models;
using DimensionService.Models.DimensionModels.FriendInfoModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace DimensionService.Hubs
{
    public class InformHub : Hub
    {
        private readonly IUserInfoDAO _userInfoDAO;
        private readonly IFriendInfoDAO _friendInfoDAO;

        public InformHub(IUserInfoDAO userInfoDAO, IFriendInfoDAO friendInfoDAO)
        {
            _userInfoDAO = userInfoDAO;
            _friendInfoDAO = friendInfoDAO;
        }

        public override Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();
            httpContext.Request.Query.TryGetValue("UserID", out StringValues userID);
            httpContext.Request.Query.TryGetValue("Token", out StringValues token);
            httpContext.Request.Query.TryGetValue("Device", out StringValues device);

            LinkInfoModel linkInfo = new()
            {
                ConnectionId = Context.ConnectionId,
                UserID = userID,
                Token = token,
                Device = device
            };
            if (ClassHelper.LinkInfos.TryAdd(Context.ConnectionId, linkInfo))
            {
                if (ClassHelper.LinkInfos.Values.Where(item => item.UserID == linkInfo.UserID).ToList() is List<LinkInfoModel> linkInfos && linkInfos.Count == 1)
                {
                    _userInfoDAO.UpdatedOnLine(linkInfo.UserID, true);
                    List<FriendModel> friends = _friendInfoDAO.GetFriends(userID);
                    List<string> connectionIds = new();
                    connectionIds.AddRange(from LinkInfoModel item in ClassHelper.LinkInfos.Values
                                           where friends.Find(friend => friend.UserID == item.UserID) != null
                                           select item.ConnectionId);
                    Clients.Client(Context.ConnectionId).SendAsync(
                        method: HubMessageType.OnlineStatus.ToString(),
                        arg1: true);
                    Clients.Clients(connectionIds).SendAsync(
                        method: HubMessageType.FriendOnline.ToString(),
                        arg1: linkInfo.UserID,
                        arg2: true);
                }
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (ClassHelper.LinkInfos.TryRemove(Context.ConnectionId, out LinkInfoModel linkInfo))
            {
                if (ClassHelper.LinkInfos.Values.FirstOrDefault(item => item.UserID == linkInfo.UserID) == null)
                {
                    _userInfoDAO.UpdatedOnLine(linkInfo.UserID, false);
                    List<FriendModel> friends = _friendInfoDAO.GetFriends(linkInfo.UserID);
                    List<string> connectionIds = new();
                    connectionIds.AddRange(from LinkInfoModel item in ClassHelper.LinkInfos.Values
                                           where friends.Find(friend => friend.UserID == item.UserID) != null
                                           select item.ConnectionId);
                    Clients.Client(Context.ConnectionId).SendAsync(
                        method: HubMessageType.OnlineStatus.ToString(),
                        arg1: false);
                    Clients.Clients(connectionIds).SendAsync(
                        method: HubMessageType.FriendOnline.ToString(),
                        arg1: linkInfo.UserID,
                        arg2: false);
                }
            }


            return base.OnDisconnectedAsync(exception);
        }
    }
}
