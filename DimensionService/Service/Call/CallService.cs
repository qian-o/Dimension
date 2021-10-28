using DimensionService.Common;
using DimensionService.Dao.CallRoom;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.CallRoomModels;
using DimensionService.Models.RequestModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DimensionService.Service.Call
{
    public class CallService : ICallService
    {
        private readonly ICallRoomDAO _callRoomDAO;

        public CallService(ICallRoomDAO callRoomDAO)
        {
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
                _callRoomDAO.UpdatedCallRoom(data.UserID, data.UseDevice, data.CallType, data.Member, true);
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
    }
}