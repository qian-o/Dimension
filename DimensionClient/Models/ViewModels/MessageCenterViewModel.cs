using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DimensionClient.Models.ViewModels
{
    public class MessageCenterViewModel : ModelBase
    {
        public ObservableCollection<ChatColumnInfoModel> ChatColumnInfos { get; set; }

        public override void InitializeVariable()
        {
            ChatColumnInfos = new ObservableCollection<ChatColumnInfoModel>();
        }
    }
}