using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Microsoft.Practices.Prism.Mvvm;
using UniversalWorldClock.ViewModels;

namespace UniversalWorldClock.Views
{
    public sealed partial class Options : IView
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
