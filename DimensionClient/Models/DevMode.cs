using System.Runtime.InteropServices;
using static DimensionClient.Common.ClassHelper;

namespace DimensionClient.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DevMode
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private readonly string dmDeviceName;
        public string DmDeviceName => dmDeviceName;

        public short DmSpecVersion { get; }

        public short DmDriverVersion { get; }

        public short DmSize { get; }

        public short DmDriverExtra { get; }

        public int DmFields { get; }

        public int DmPositionX { get; }

        public int DmPositionY { get; }

        public Orientation DmDisplayOrientation { get; }

        public int DmDisplayFixedOutput { get; }

        public short DmColor { get; }

        public short DmDuplex { get; }

        public short DmYResolution { get; }

        public short DmTTOption { get; }

        public short DmCollate { get; }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private readonly string dmFormName;
        public string DmFormName => dmFormName;

        public short DmLogPixels { get; }

        public int DmBitsPerPel { get; }

        public int DmPelsWidth { get; }

        public int DmPelsHeight { get; }

        public int DmDisplayFlags { get; }

        public int DmDisplayFrequency { get; }

        public int DmICMMethod { get; }

        public int DmICMIntent { get; }

        public int DmMediaType { get; }

        public int DmDitherType { get; }

        public int DmReserved1 { get; }

        public int DmReserved2 { get; }

        public int DmPanningWidth { get; }

        public int DmPanningHeight { get; }
    }
}
