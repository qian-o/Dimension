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
            DataContext = data;
            CallVideoData = data;
        }
    }
}
