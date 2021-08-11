using DimensionClient.Common;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace DimensionClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (ThreadPool.SetMaxThreads(25, 25))
            {
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            };

            if (SystemParameters.MenuDropAlignment)
            {
                FieldInfo field = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
                field.SetValue(null, false);
            }

            FontFamily fontFamily = new(new Uri("pack://application:,,,/"), "/Library/Font/苹方黑体-中粗-简.ttf#.萍方-简");
            Control.FontFamilyProperty.OverrideMetadata(typeof(TextBoxBase), new FrameworkPropertyMetadata(fontFamily));
            Control.FontFamilyProperty.OverrideMetadata(typeof(ContentControl), new FrameworkPropertyMetadata(fontFamily));
            TextElement.FontFamilyProperty.OverrideMetadata(typeof(TextElement), new FrameworkPropertyMetadata(fontFamily));
            TextBlock.FontFamilyProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(fontFamily));
            FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(Control), new FrameworkPropertyMetadata(defaultValue: null));
            ClassHelper.Dispatcher = Dispatcher;

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ClassHelper.RecordException(typeof(App), e.Exception);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ClassHelper.RecordException(typeof(App), e.ExceptionObject as Exception);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ClassHelper.RecordException(typeof(App), e.Exception);
            e.SetObserved();
        }
    }
}
