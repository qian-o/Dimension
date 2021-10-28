using DimensionService.Common;
using DimensionService.Dao.CallRecord;
using DimensionService.Dao.CallRoom;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.CallRoomModels;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DimensionService.Service.Call
{
    public class CallService : ICallService
    {
        private readonly ICallRoomDAO _callRoomDAO;
        private readonly ICallRecordDAO _callRecordDAO;

        public CallService(ICallRoomDAO callRoomDAO, ICallRecordDAO callRecordDAO)
        {
            _callRoomDAO = callRoomDAO;
            _callRecordDAO = callRecordDAO;
        }

        public bool CreateCall(CreateCallModel data, out RoomPermissionInfoModel roomPermission, out string message)
        {
            try
            {
                bool state = false;
                roomPermission = new RoomPermissionInfoModel();
                message = string.Empty;
                if (!_callRecordDAO.EffectiveCallRecord(data.UserID))
                {
                    if (data.Member.Count > 0)
                    {
                        roomPermission.RoomID = _callRoomDAO.GetCallRoomForHouseOwnerID(data.UserID).RoomID;
                        if (_callRoomDAO.GetRoomStatus(data.UserID))
                        {
                            ClassHelper.DissolutionRoom(ClassHelper.callAppID, roomPermission.RoomID, out string _);
                        }
                        _callRoomDAO.UpdatedCallRoom(data.UserID, data.CallType, data.Member, true);
                        roomPermission.UserSig = ClassHelper.GetCallAuthorization(data.UserID, roomPermission.RoomID, data.CallType, createRoom: true);
                        state = true;
                    }
                    else
                    {
                        message = "人员不能为空。";
                    }
                }
                else
                {
                    message = "请先结束当前通话。";
                }
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetUserSig(GetUserSigModel data, out string userSig, out string message)
        {
            try
            {
                bool state = false;
                userSig = string.Empty;
                message = string.Empty;
                if (!_callRecordDAO.EffectiveCallRecord(data.UserID))
                {
                    if (_callRoomDAO.GetCallRoomForRoomID(data.RoomID) is CallRoomModel callRoom)
                    {
                        if (callRoom.Enabled)
                        {
                            List<RoommateModel> roommates = JsonConvert.DeserializeObject<List<RoommateModel>>(callRoom.Roommate);
                            if (roommates.FirstOrDefault(item => item.UserID == data.UserID) is RoommateModel roommate)
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
                    }
                    else
                    {
                        message = "房间不存在。";
                    }
                }
                else
                {
                    message = "请先结束当前通话。";
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