using System.Collections.Generic;

namespace DimensionService.Models.ResultModels
{
    public class FriendSortModel
    {
        public string Sort { get; set; }
        public List<FriendBriefModel> FriendBriefs { get; set; }
    }
}
