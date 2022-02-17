using System.Windows;
using System.Windows.Input;

namespace DimensionClient.Models
{
    public class PointerEventModel
    {
        public EventHandler EventHandler { get; set; }
        public MouseButtonEventHandler MouseButtonEvent { get; set; }
        public EventHandler<TouchEventArgs> TouchEvent { get; set; }

        public static PointerEventModel Instance(EventHandler handler)
        {
            return new PointerEventModel
            {
                EventHandler = handler,
                MouseButtonEvent = (object sender, MouseButtonEventArgs e) =>
                {
                    if (sender is UIElement element)
                    {
                        if (e.StylusDevice == null)
                        {
                            handler.Invoke(sender, e);
                        }
                    }
                },
                TouchEvent = (object sender, TouchEventArgs e) =>
                {
                    handler.Invoke(sender, e);
                }
            };
        }
    }
}
