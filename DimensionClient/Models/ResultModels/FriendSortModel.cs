using System.Collections.Generic;

namespace DimensionClient.Models.ResultModels
{
    public class FriendSortModel
    {
        public string Sort { get; set; }
        public List<FriendBriefModel> FriendBriefs { get; set; }
    }
}
