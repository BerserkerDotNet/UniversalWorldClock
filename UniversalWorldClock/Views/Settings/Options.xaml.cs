using UniversalWorldClock.Common;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace UniversalWorldClock.Views.Settings
{
    public sealed partial class Options : UserControl
    {
        public Options()
        {
            this.InitializeComponent();
        }

        private void OnBackClicked(object sender, RoutedEventArgs e)
        {
            SettingsPane.Show();
        }
    }
}
