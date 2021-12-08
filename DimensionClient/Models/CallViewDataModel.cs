using DimensionClient.Common;
using ManageLiteAV;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DimensionClient.Models
{
    public class CallViewDataModel : ModelBase, ITRTCVideoRenderCallback
    {
        private readonly object dataObject = new();
        private WriteableBitmap writeable;
        private Int32Rect int32Rect;
        private bool isVideo;
        private bool isAudio;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 可写位图数据
        /// </summary>
        public WriteableBitmap Writeable
        {
            get => writeable;
            set
            {
                writeable = value;
                OnPropertyChanged(nameof(Writeable));
            }
        }

        /// <summary>
        /// 位图分辨率
        /// </summary>
        public Int32Rect Int32Rect
        {
            get => int32Rect;
            set
            {
                int32Rect = value;
                OnPropertyChanged(nameof(Int32Rect));
            }
        }

        /// <summary>
        /// 是否进入房间
        /// </summary>
        public bool? IsEnter { get; set; }

        /// <summary>
        /// 是否显示视频
        /// </summary>
        public bool IsVideo
        {
            get => isVideo;
            set
            {
                isVideo = value;
                OnPropertyChanged(nameof(IsVideo));
            }
        }

        /// <summary>
        /// 是否播放音频
        /// </summary>
        public bool IsAudio
        {
            get => isAudio;
            set
            {
                isAudio = value;
                OnPropertyChanged(nameof(IsAudio));
            }
        }

        public override void InitializeVariable()
        {
            UserID = string.Empty;
            writeable = null;
            int32Rect = new Int32Rect();
            IsEnter = null;
            isVideo = false;
            isAudio = false;
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
