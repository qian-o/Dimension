using DimensionService.Common;
using DimensionService.Dao.CallRoom;
using DimensionService.Hubs;
using DimensionService.Models;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.CallRoomModels;
using DimensionService.Models.RequestModels;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DimensionService.Service.Call
{
    public class CallService : ICallService
    {
        private readonly IHubContext<InformHub> _hub;
        private readonly ICallRoomDAO _callRoomDAO;

        public CallService(IHubContext<InformHub> hub, ICallRoomDAO callRoomDAO)
        {
            _hub = hub;
            _callRoomDAO = callRoomDAO;
        }

        public bool CreateCall(CreateCallModel data, out string roomID, out string message)
        {
            try
            {
                bool state = false;
                roomID = string.Empty;
                message = string.Empty;
                CallRoomModel callRoom = _callRoomDAO.GetCallRoomForHouseOwner(data.UserID, data.UseDevice);
                if (callRoom.Enabled)
                {
                    ClassHelper.DissolutionRoom(ClassHelper.callAppID, callRoom.RoomID, out string _);
                }
                _callRoomDAO.UpdateCallRoom(data.UserID, data.UseDevice, data.CallType, data.Member, true);
                roomID = callRoom.RoomID;
                state = true;
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetUserSig(string userID, string roomID, out string userSig, out string message)
        {
            try
            {
                bool state = false;
                userSig = string.Empty;
                message = string.Empty;
                CallRoomModel callRoom = _callRoomDAO.GetCallRoomForRoomID(roomID);
                if (callRoom.Enabled)
                {
                    List<RoommateModel> roommates = JsonConvert.DeserializeObject<List<RoommateModel>>(callRoom.Roommate);
                    if (roommates.FirstOrDefault(item => item.UserID == userID) is RoommateModel roommate)
                    {
                        userSig = roommate.UserSig;
                        state = true;
                    }
                    else
                    {
                        message = "您不属于当前房间成员。";
                    }
                }
                else
                {
                    message = "房间未启用。";
                }
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool NotifyRoommate(string userID, ClassHelper.UseDevice useDevice, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                CallRoomModel callRoom = _callRoomDAO.GetCallRoomForHouseOwner(userID, useDevice);
                if (callRoom.Enabled)
                {
                    List<RoommateModel> roommates = JsonConvert.DeserializeObject<List<RoommateModel>>(callRoom.Roommate);
                    foreach (RoommateModel roommate in roommates)
                    {
                        if (roommate.UserID != userID)
                        {
                            foreach (LinkInfoModel linkInfo in ClassHelper.LinkInfos.Values.Where(item => item.UserID == roommate.UserID))
                            {
                                _hub.Clients.Client(linkInfo.ConnectionId).SendAsync(method: ClassHelper.HubMessageType.CallInvite.ToString(),
                                                                                     arg1: callRoom.RoomID);
                            }
                        }
                    }
                }
                else
                {
                    message = "房间未启用。";
                }
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ReplyCall(ReplyCallModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                CallRoomModel callRoom = _callRoomDAO.GetCallRoomForRoomID(data.RoomID);
                if (callRoom.Enabled)
                {
                    if (_callRoomDAO.UpdateRoommateStatus(data.UserID, data.RoomID, data.IsAcceptCall, out message))
                    {
                        if (data.UserID != callRoom.HouseOwnerID)
                        {
                            foreach (LinkInfoModel linkInfo in ClassHelper.LinkInfos.Values.Where(item => item.UserID == callRoom.HouseOwnerID && item.Device == callRoom.HouseOwnerDevice.ToString()))
                            {
                                _hub.Clients.Client(linkInfo.ConnectionId).SendAsync(method: ClassHelper.HubMessageType.AcceptCall.ToString(),
                                                                                     arg1: data.IsAcceptCall);
                            }
                        }
                    }
                }
                else
                {
                    message = "房间未启用。";
                }
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DissolutionRoom(string userID, ClassHelper.UseDevice useDevice, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                CallRoomModel callRoom = _callRoomDAO.GetCallRoomForHouseOwner(userID, useDevice);
                ClassHelper.DissolutionRoom(ClassHelper.callAppID, callRoom.RoomID, out string _);
                _callRoomDAO.UpdateCallRoom(userID, useDevice, null, null, false);
                state = true;
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}