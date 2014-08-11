using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

namespace UniversalWorldClock.Tasks
{
    public sealed class TileSchedulerTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            var updater = new LiveTileScheduler();
            await updater.CreateSchedule();
            deferral.Complete();
        }
    }
}