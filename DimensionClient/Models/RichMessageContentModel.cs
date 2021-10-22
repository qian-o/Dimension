using DimensionClient.Common;

namespace DimensionClient.Models
{
    public class RichMessageContentModel
    {
        public ClassHelper.RichMessageType MessageType { get; set; }
        public string Content { get; set; }
        public FileModel FileAttribute { get; set; }
    }
}
