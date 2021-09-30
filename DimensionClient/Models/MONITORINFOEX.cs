using System.Runtime.InteropServices;

namespace DimensionClient.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class MONITORINFOEX
    {
        public int CbSize { get; } = Marshal.SizeOf(typeof(MONITORINFOEX));

        public RECT RcMonitor { get; }

        public RECT RcWork { get; }

        public int DwFlags { get; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        private readonly char[] szDevice = new char[32];
        public char[] SzDevice => szDevice;
    }
}
