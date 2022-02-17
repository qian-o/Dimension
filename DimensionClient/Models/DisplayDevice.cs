using System.Runtime.InteropServices;

namespace DimensionClient.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DisplayDevice
    {
        [MarshalAs(UnmanagedType.U4)]
        private int cbSize;
        public int CbSize { get => cbSize; set => cbSize = value; }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private readonly string deviceName;
        public string DeviceName => deviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        private readonly string deviceString;
        public string DeviceString => deviceString;

        [MarshalAs(UnmanagedType.U4)]
        private readonly DisplayDeviceState deviceState;
        public DisplayDeviceState DeviceState => deviceState;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        private readonly string deviceID;
        public string DeviceID => deviceID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        private readonly string deviceKey;
        public string DeviceKey => deviceKey;

        [Flags]
        public enum DisplayDeviceState
        {
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            PrimaryDevice = 0x4,
            MirroringDriver = 0x8,
            VGACompatible = 0x10,
            Removable = 0x20,
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }
    }
}