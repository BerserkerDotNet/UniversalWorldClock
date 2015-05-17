using System;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using UniversalWorldClock.Common;
using UniversalWorldClock.ViewModels;
#if !WINDOWS_PHONE_APP
using Windows.UI.ApplicationSettings;
#endif

namespace UniversalWorldClock.Views
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("UC_Setting", "Options", ClockSettings));
            args.Request.ApplicationCommands.Add(new SettingsCommand("UC_Privacy", "Privacy Policy", PrivacyPolicy));
            args.Request.ApplicationCommands.Add(new SettingsCommand("UC_Donate", "Donate", Donate));
        }

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
            await Launcher.LaunchUriAsync(new Uri("http://sdrv.ms/1fReV4E"));
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
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= MainPage_CommandsRequested;
            base.OnNavigatedFrom(e);
        }

        protected override string DetermineVisualState(double width, double height)
        {
            return ViewStateHelper.GetViewState(width).ToString();
        }

        private void SearchBox_OnSuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            ((MainPageViewModel)DataContext ).OnSearchPaneSuggestionsRequested(sender, args);
        }

        private void SearchBox_OnQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            ((MainPageViewModel)DataContext).OnSearchPaneQuerySubmitted(sender, args);
            sender.QueryText = "";
        }

        private void SearchBox_OnResultSuggestionChosen(SearchBox sender, SearchBoxResultSuggestionChosenEventArgs args)
        {
            ((MainPageViewModel)DataContext).OnSearchPaneResultSuggestionChosen(sender, args);
            sender.QueryText = "";
        }
    }
}
