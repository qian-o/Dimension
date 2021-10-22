using DimensionClient.Common;
using System.Collections.Generic;

namespace DimensionClient.Models
{
    public class RichMessageModel
    {
        public string SerializedMessage { get; set; }
        public List<RichMessageContentModel> RichMessageContents { get; set; } = new List<RichMessageContentModel>();

        /// <summary>
        /// 筛选富文本消息列表
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns></returns>
        public List<RichMessageContentModel> Filter(ClassHelper.RichMessageType type)
        {
            return RichMessageContents.FindAll(item => item.MessageType == type);
        }
    }
}
