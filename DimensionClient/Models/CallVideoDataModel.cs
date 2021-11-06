using DimensionClient.Common;
using ManageLiteAV;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DimensionClient.Models
{
    public class CallVideoDataModel : ModelBase, ITRTCVideoRenderCallback
    {
        private readonly object dataObject = new();
        private WriteableBitmap writeable;

        public string UserID { get; set; }
        public WriteableBitmap Writeable
        {
            get => writeable;
            set
            {
                writeable = value;
                OnPropertyChanged(nameof(Writeable));
            }
        }
        public Int32Rect Int32Rect { get; set; }
        public bool? IsEnter { get; set; }

        public override void InitializeVariable()
        {
            UserID = string.Empty;
            writeable = null;
            Int32Rect = new Int32Rect();
            IsEnter = null;
        }

        public void onRenderVideoFrame(string userId, TRTCVideoStreamType streamType, TRTCVideoFrame frame)
        {
            try
            {
                lock (dataObject)
                {
                    if (Writeable == null)
                    {
                        ClassHelper.Dispatcher.Invoke(delegate
                        {
                            Writeable = new WriteableBitmap((int)frame.width, (int)frame.height, 96, 96, PixelFormats.Pbgra32, null);
                            Int32Rect = new Int32Rect(0, 0, (int)frame.width, (int)frame.height);
                        });
                    }
                    ClassHelper.Dispatcher.Invoke(delegate
                    {
                        Writeable.Lock();
                        Marshal.Copy(frame.data, 0, Writeable.BackBuffer, frame.data.Length);
                        Writeable.AddDirtyRect(Int32Rect);
                        Writeable.Unlock();
                    });
                }
            }
            catch (TaskCanceledException)
            {
                // 任务取消，不算异常。
            }
        }
    }
}
