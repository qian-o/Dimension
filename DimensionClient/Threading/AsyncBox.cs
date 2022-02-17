using DimensionClient.Library.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DimensionClient.Threading
{
    public class AsyncBox : FrameworkElement
    {
        private readonly HostVisual _hostVisual = new();

        public AsyncBox()
        {
            Thread thread = new(() =>
            {
                Loading loading = new()
                {
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6D00"))
                };
                VisualTargetSource _visualTarget = new(_hostVisual)
                {
                    RootVisual = loading
                };
                Dispatcher.Run();
            })
            {
                IsBackground = true
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        protected override Visual GetVisualChild(int index)
        {
            return _hostVisual;
        }

        protected override int VisualChildrenCount => 1;
    }
}