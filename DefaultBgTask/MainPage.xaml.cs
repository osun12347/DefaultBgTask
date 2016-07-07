using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DefaultBgTask
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string _progress = "";
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var task= BgTool.RegisterBackgroundTask("Task.BackgroundTask", "BackgroundTask", new SystemTrigger(SystemTriggerType.TimeZoneChange, false), null);
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
            task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
        }

        private void OnProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            Debug.WriteLine("Onprogress.........................................");
           var ignored= Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var progress = "Progress:" + args.Progress + "%";
                _progress = progress;
                UpdateUI();
            });
        }

        private void OnCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            UpdateUI();

        }

    private async void UpdateUI()
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                tb1.Text = _progress;
                var settings = ApplicationData.Current.LocalSettings;
                tb2.Text = settings.Values.ContainsKey("taskStatus") ? settings.Values["taskStatus"].ToString() : "";
                settings.Values["taskStatus"] = null;
            });
        }
    }
}
