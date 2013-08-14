using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels;
using UniversalWorldClock.Views.Settings;
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
            args.Request.ApplicationCommands.Add(new SettingsCommand("UC_Privacy", " Privacy Policy", PrivacyPolicy));
        }

        private void ClockSettings(IUICommand command)
        {
            OpenPopup(346, new Options());
        }

        private void PrivacyPolicy(IUICommand command)
        {
            OpenPopup(600, new Privacy());
        }

        private void OpenPopup(int width, UserControl child)
        {
            var settingsPopup = new Popup
            {
                IsLightDismissEnabled = true,
                Width = width,
                Height = Window.Current.Bounds.Height
            };

            child.Width = width;
            child.Height = Window.Current.Bounds.Height;
            settingsPopup.Child = child;
            settingsPopup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - width);
            settingsPopup.SetValue(Canvas.TopProperty, 0);
            settingsPopup.IsOpen = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= MainPage_CommandsRequested;
            base.OnNavigatedFrom(e);
        }
    }
}
