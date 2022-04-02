using Dimension.Domain;
using DimensionClient.Common;
using DimensionClient.Models.ResultModels;

namespace DimensionClient.Models.ViewModels
{
    public class InviteCallViewModel : ModelBase
    {
        private FriendDetailsModel friendDetails;
        private CallType callType;
        private string roomID;

        public FriendDetailsModel FriendDetails
        {
            get => friendDetails;
            set
            {
                friendDetails = value;
                OnPropertyChanged(nameof(FriendDetails));
            }
        }
        public CallType CallType
        {
            get => callType;
            set
            {
                callType = value;
                OnPropertyChanged(nameof(CallType));
            }
        }
        public string RoomID
        {
            get => roomID;
            set
            {
                roomID = value;
                OnPropertyChanged(nameof(RoomID));
            }
        }

        public override void InitializeVariable()
        {
            friendDetails = null;
            callType = CallType.Voice;
            roomID = string.Empty;
        }
    }
}
