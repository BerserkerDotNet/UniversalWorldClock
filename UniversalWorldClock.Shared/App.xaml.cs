using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
#if WINDOWS_PHONE_APP
    using Windows.Phone.UI.Input;
#endif
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using UniversalWorldClock.Common;
using UniversalWorldClock.Data;
using UniversalWorldClock.Services;
using UniversalWorldClock.Tasks;
using UniversalWorldClock.ViewModels;
using UniversalWorldClock.ViewModels.Interfaces;
using UniversalWorldClock.Views;
#if !WINDOWS_PHONE_APP
using Windows.ApplicationModel.Search;
#endif

namespace UniversalWorldClock
{
    sealed partial class App
    {
        private static readonly UnityContainer Container = new UnityContainer();
        private const string TASKNAMEUSERPRESENT = "TileSchedulerTaskUserPresent";
        private const string TASKNAMETIMER = "TileSchedulerTaskTimer";
        private const string TASKENTRYPOINT = "UniversalWorldClock.Tasks.TileSchedulerTask";

        public App()
        {
            this.InitializeComponent();
#if !WINDOWS_PHONE_APP
            ExtendedSplashScreenFactory = sp => new SplashScreenPage(sp);
#endif
        }

        public bool SuppressBackButton { get; set; }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container.RegisterInstance(NavigationService);
            Container.RegisterInstance(SessionStateService);
            Container.RegisterType<IClock, DefaultClock>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IMainPageViewModel, MainPageViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataRepository, DataRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            Container.RegisterType<TimeShiftProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICacheClient, SimpleMemoryCacheClient>(new ContainerControlledLifetimeManager());
            return Task.FromResult<object>(null);
        }

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            await InitializeData();
            NavigationService.Navigate(Experiences.Main.ToString(), null);
        }

        protected override object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static T Resolve<T>(params Tuple<string, object>[] arguments)
        {
            var overrides = arguments.Select(t => new ParameterOverride(t.Item1, t.Item2))
                .Cast<ResolverOverride>()
                .ToArray();
            return Container.Resolve<T>(overrides);
        }

        private async Task InitializeData()
        {
            await DataRepository.LoadCities();
            await CreateTileUpdateTask();
            var updater = new LiveTileScheduler();
            await updater.CreateSchedule();
        }

        private static async Task CreateTileUpdateTask()
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
#if WINDOWS_PHONE_APP
        protected override void OnHardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            if (!SuppressBackButton)
                base.OnHardwareButtonsBackPressed(sender, e);
        }
#endif
    }
}
