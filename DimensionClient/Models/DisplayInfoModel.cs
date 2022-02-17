namespace DimensionClient.Models
{
    public class DisplayInfoModel
    {
        public IntPtr WindowIntPtr { get; set; }
        public int DisplayWidth { get; set; }
        public int DisplayHeight { get; set; }
        public int DisplayLeft { get; set; }
        public int DisplayTop { get; set; }
        public bool MainDisplay { get; set; }
        public double ShowWidth { get; set; }
        public double ShowHeight { get; set; }
        public double ShowLeft { get; set; }
        public double ShowTop { get; set; }
    }
}
