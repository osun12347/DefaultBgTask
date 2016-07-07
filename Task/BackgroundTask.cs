using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System.Threading;

namespace Task
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        IBackgroundTaskInstance _taskInstance;
        BackgroundTaskDeferral _deferral;
        BackgroundTaskCancellationReason _reason = BackgroundTaskCancellationReason.Abort;
        bool _isCanceled = false;
        ThreadPoolTimer _timer = null;
        uint _progress = 0;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Run.............");
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["BackgroundWorkCost"] = cost.ToString();
            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
            _timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(MyTimerCallback), TimeSpan.FromSeconds(1));
        }

        private void MyTimerCallback(ThreadPoolTimer timer)
        {
            if ((_isCanceled == false) && (_progress < 100))
            {
                _progress += 10;
                _taskInstance.Progress = _progress;
            }
            else
            {
                _timer.Cancel();
                //var key = _taskInstance.Task.Name;
                String taskStatus = (_progress < 100) ? "Canceled with reason:" + _reason.ToString() : "Completed";
                var settings = ApplicationData.Current.LocalSettings;
                settings.Values["taskStatus"] = taskStatus;
                //Debug.WriteLine("Background " + _taskInstance.Task.Name + settings.Values[key]);
                _deferral.Complete();
            }
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _isCanceled = true;
            _reason = reason;
        }
    }
}
