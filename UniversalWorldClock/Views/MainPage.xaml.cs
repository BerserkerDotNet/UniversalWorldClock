using System;
using UniversalWorldClock.Common;
using UniversalWorldClock.Views.Settings;
using Windows.System;
 #if !WINDOWS_PHONE_APP
using Windows.UI.ApplicationSettings;
#endif
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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

        #if !WINDOWS_PHONE_APP
        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("UC_Setting", "Options", ClockSettings));
            args.Request.ApplicationCommands.Add(new SettingsCommand("UC_Privacy", "Privacy Policy", PrivacyPolicy));
            args.Request.ApplicationCommands.Add(new SettingsCommand("UC_Donate", "Donate", Donate));
        }
#endif

        private void Donate(IUICommand command)
        {
            Launcher.LaunchUriAsync(new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=UFS2JX3EJGU3N"));
        }

        private void ClockSettings(IUICommand command)
        {
            OpenPopup(346, new Options());
        }

        private async void PrivacyPolicy(IUICommand command)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://sdrv.ms/1fReV4E"));
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
            #if !WINDOWS_PHONE_APP
            if (ApplicationView.Value != ApplicationViewState.Snapped)
            {
                uiCurrentLocation.Margin = new Thickness(Window.Current.Bounds.Width - 1000, 0, 0, 0);
            }
            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
#endif
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
#if !WINDOWS_PHONE_APP
            SettingsPane.GetForCurrentView().CommandsRequested -= MainPage_CommandsRequested;
#endif
            base.OnNavigatedFrom(e);
        }

        protected override void OnSizeChanged(ApplicationView view)
        {
            if (CurrentViewState != ViewState.Snapped)
            {
                uiCurrentLocation.Margin = new Thickness(Window.Current.Bounds.Width - 1000, 0, 0, 0);
            }
            base.OnSizeChanged(view);
        }
    }
}
