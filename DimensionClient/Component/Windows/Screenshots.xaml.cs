using DimensionClient.Common;
using DimensionClient.Models.ViewModels;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace DimensionClient.Component.Windows
{
    /// <summary>
    /// Screenshots.xaml 的交互逻辑
    /// </summary>
    public partial class Screenshots : Window
    {
        private readonly ScreenshotsViewModel mainData;

        // 原图
        private readonly Bitmap artwork;
        // X缩放
        private readonly double zoomX;
        // y缩放
        private readonly double zoomY;
        // 是否保存
        public bool IsSave { get; private set; }

        public Screenshots(Bitmap bitmap, double showLeft, double showTop, double showWidth, double showHeight)
        {
            InitializeComponent();

            Left = showLeft;
            Top = showTop;
            Width = showWidth;
            Height = showHeight;

            artwork = bitmap;
            zoomX = bitmap.Width / Width;
            zoomY = bitmap.Height / Height;
            IntPtr delPtr = bitmap.GetHbitmap();
            imgMain.Source = Imaging.CreateBitmapSourceFromHBitmap(delPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ClassHelper.DeleteIntPtr(delPtr);

            mainData = DataContext as ScreenshotsViewModel;
        }

        private void ScreenshotsMain_Loaded(object sender, RoutedEventArgs e)
        {
            lin1.Point = new(Width, 0);
            lin2.Point = new(Width, Height);
            lin3.Point = new(0, Height);
            lin4.Point = new(0, 0);
        }

        private void ScreenshotsMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!mainData.PointStart.Equals(mainData.Point1))
                {
                    mainData.PointStart = new(0, 0);
                    mainData.Point1 = new(0, 0);
                    mainData.Point2 = new(0, 0);
                    mainData.Point3 = new(0, 0);
                    mainData.Point4 = new(0, 0);

                    pthSelect.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Close();
                }
            }
        }

        private void GrdMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);
            mainData.PointStart = point;
            mainData.Point1 = point;
            mainData.Point2 = point;
            mainData.Point3 = point;
            mainData.Point4 = point;

            pthSelect.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Collapsed;
        }

        private void GrdMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(this);
                mainData.Point1 = new(point.X, mainData.PointStart.Y);
                mainData.Point2 = point;
                mainData.Point3 = new(mainData.PointStart.X, point.Y);
            }
        }

        private void GrdMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mainData.PointStart.Equals(mainData.Point1))
            {
                Point point = mainData.MaxPoint();
                btnSave.Margin = point.X > (btnSave.Width - 5) && point.Y > (btnSave.Height - 5)
                    ? (new(point.X - btnSave.Width - 5, point.Y - btnSave.Height - 5, 0, 0))
                    : (new(point.X + 5, point.Y + 5, 0, 0));
                btnSave.Visibility = Visibility.Visible;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Point minPoint = mainData.MinPoint();
            int minWidth = Convert.ToInt32(minPoint.X * zoomX);
            int minHeight = Convert.ToInt32(minPoint.Y * zoomY);
            Point maxPoint = mainData.MaxPoint();
            int maxWidth = Convert.ToInt32((maxPoint.X - minPoint.X) * zoomX);
            int maxHeight = Convert.ToInt32((maxPoint.Y - minPoint.Y) * zoomY);
            Bitmap bitmap = new(maxWidth, maxHeight);
            Graphics.FromImage(bitmap).DrawImage(artwork, new Rectangle(0, 0, maxWidth, maxHeight), new(minWidth, minHeight, maxWidth, maxHeight), GraphicsUnit.Pixel);
            IntPtr delPtr = bitmap.GetHbitmap();
            Clipboard.SetImage(Imaging.CreateBitmapSourceFromHBitmap(delPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
            ClassHelper.DeleteIntPtr(delPtr);

            bitmap.Dispose();
            Close();
            IsSave = true;
        }
    }
}
