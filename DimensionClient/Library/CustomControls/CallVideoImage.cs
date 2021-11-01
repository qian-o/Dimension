using DimensionClient.Models.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DimensionClient.Library.CustomControls
{
    public class CallVideoImage : Image
    {
        public CallVideoImageViewModel CallVideoData { get; private set; }

        public CallVideoImage(CallVideoImageViewModel data)
        {
            _ = SetBinding(SourceProperty, new Binding { Path = new PropertyPath("Writeable") });
            Stretch = System.Windows.Media.Stretch.UniformToFill;
            DataContext = data;
            CallVideoData = data;
        }
    }
}
