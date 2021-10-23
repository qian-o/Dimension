using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace DimensionClient.Library.CustomControls
{
    public class DesignerImage : Image
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageSource Source
        {
            get => base.Source;
            set => base.Source = value;
        }
    }
}
