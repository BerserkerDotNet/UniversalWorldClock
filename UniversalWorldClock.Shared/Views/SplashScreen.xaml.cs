using System;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using UniversalWorldClock.Data;
using UniversalWorldClock.Tasks;

namespace UniversalWorldClock.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreen
    {
        private const string TASKNAMEUSERPRESENT = "TileSchedulerTaskUserPresent";
        private const string TASKNAMETIMER = "TileSchedulerTaskTimer";
        private const string TASKENTRYPOINT = "UniversalWorldClock.Tasks.TileSchedulerTask";

        public SplashScreen()
        {
            this.InitializeComponent();

            InitializeData();
        }

        private async void InitializeData()
        {
            await DataRepository.LoadCities();
            CreateTileUpdateTask();
            var updater = new LiveTileScheduler();
            await updater.CreateSchedule();

            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            frame.Navigate(typeof(MainPage), string.Empty);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private static async void CreateTileUpdateTask()
        {

            try
            {
                var result = await BackgroundExecutionManager.RequestAccessAsync();
                if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                    result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {
                    EnsureUserPresentTask();
                    EnsureTimerTask();
                }
            }
            catch (Exception)
            {
                //You can’t change background task and lock screen privileges while running this application in the simulator.
            }

        }

        private static void EnsureUserPresentTask()
        {
            if (BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == TASKNAMEUSERPRESENT))
                return;

            var builder = new BackgroundTaskBuilder();
            builder.Name = TASKNAMEUSERPRESENT;
            builder.TaskEntryPoint = TASKENTRYPOINT;
            builder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
            builder.Register();
        }

        private static void EnsureTimerTask()
        {
            if (BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == TASKNAMETIMER))
                return;

            var builder = new BackgroundTaskBuilder();
            builder.Name = TASKNAMETIMER;
            builder.TaskEntryPoint = TASKENTRYPOINT;
            builder.SetTrigger(new TimeTrigger(20, false));
            var result = builder.Register();
        }
    }
}
