using DimensionClient.Models;
using System.Windows;

namespace DimensionClient.Common
{
    public static class PointerEventHelper
    {
        private static readonly List<PointerEventModel> pointerUpEvents = new();
        private static readonly List<PointerEventModel> pointerDownEvents = new();

        #region PointerUp
        public static readonly RoutedEvent PointerUpEvent = EventManager.RegisterRoutedEvent("PointerUp", RoutingStrategy.Direct, typeof(EventHandler), typeof(UIElement));

        public static void AddPointerUpHandler(this UIElement element, EventHandler handler)
        {
            lock (pointerUpEvents)
            {
                PointerEventModel pointer = PointerEventModel.Instance(handler);
                element.MouseLeftButtonUp += pointer.MouseButtonEvent;
                element.TouchUp += pointer.TouchEvent;
                pointerUpEvents.Add(pointer);
            }
        }
        public static void RemovePointerUpHandler(this UIElement element, EventHandler handler)
        {
            lock (pointerUpEvents)
            {
                if (pointerUpEvents.Find(item => item.EventHandler.Equals(handler)) is PointerEventModel pointer)
                {
                    element.MouseLeftButtonUp -= pointer.MouseButtonEvent;
                    element.TouchUp -= pointer.TouchEvent;
                    pointerUpEvents.Remove(pointer);
                }
            }
        }
        #endregion

        #region PointerDown
        public static readonly RoutedEvent PointerDownEvent = EventManager.RegisterRoutedEvent("PointerDown", RoutingStrategy.Direct, typeof(EventHandler), typeof(UIElement));

        public static void AddPointerDownHandler(this UIElement element, EventHandler handler)
        {
            lock (pointerDownEvents)
            {
                PointerEventModel pointer = PointerEventModel.Instance(handler);
                element.MouseLeftButtonDown += pointer.MouseButtonEvent;
                element.TouchDown += pointer.TouchEvent;
                pointerDownEvents.Add(pointer);
            }
        }
        public static void RemovePointerDownHandler(this UIElement element, EventHandler handler)
        {
            lock (pointerDownEvents)
            {
                if (pointerDownEvents.Find(item => item.EventHandler.Equals(handler)) is PointerEventModel pointer)
                {
                    element.MouseLeftButtonDown -= pointer.MouseButtonEvent;
                    element.TouchDown -= pointer.TouchEvent;
                    pointerDownEvents.Remove(pointer);
                }
            }
        }
        #endregion
    }
}