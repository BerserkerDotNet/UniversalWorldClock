using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalWorldClock.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("UC_Setting", " Options", ClockSettings));
        }

        private void ClockSettings(IUICommand command)
        {
            const int settingsWidth = 346;
            var settingsPopup = new Popup();

            settingsPopup.IsLightDismissEnabled = true;
            settingsPopup.Width = settingsWidth;
            settingsPopup.Height = Window.Current.Bounds.Height;

            var mypane = new Settings.Options();
            mypane.Width = settingsWidth;
            mypane.Height = Window.Current.Bounds.Height;
            settingsPopup.Child = mypane;
            settingsPopup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - settingsWidth);
            settingsPopup.SetValue(Canvas.TopProperty, 0);
            settingsPopup.IsOpen = true;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= MainPage_CommandsRequested;
            base.OnNavigatedFrom(e);
        }

        private void OnDoubleTapClock(object sender, DoubleTappedRoutedEventArgs e)
        {
            var clock = (e.OriginalSource as FrameworkElement).DataContext as ClockInfo;

            (DataContext as MainViewModel).DeleteClock(clock);
        }
    }
}
