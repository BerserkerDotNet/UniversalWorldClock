using UniversalWorldClock.Common;
 #if !WINDOWS_PHONE_APP
using Windows.UI.ApplicationSettings;
#endif
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
             #if !WINDOWS_PHONE_APP
            SettingsPane.Show();
#endif
        }
    }
}
