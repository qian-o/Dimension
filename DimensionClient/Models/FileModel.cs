using DimensionClient.Common;

namespace DimensionClient.Models
{
    public class FileModel
    {
        public ClassHelper.FileType FileType { get; set; }
        public string FileName { get; set; }
        public double FileMByte { get; set; }
        public double FileWidth { get; set; }
        public double FileHeight { get; set; }
    }
}