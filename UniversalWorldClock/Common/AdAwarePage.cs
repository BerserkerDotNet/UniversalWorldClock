using LeadBolt.Windows8.AppAd;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UniversalWorldClock.Common
{
    public class AdAwarePage : LayoutAwarePage
    {
        private AdController _adController;
        private Grid _adWrapperGrid;

        protected override void OnSizeChanged(ApplicationViewState viewState)
        {
            LoadAd(viewState);
        }

        private void LoadAd(ApplicationViewState viewState)
        {
            if (viewState == ApplicationViewState.Snapped)
                LoadAd("198903296");
            else
                DestroyAd();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var adGrid = this.Content as Grid;
            if (adGrid != null && adGrid.Name.Equals("_adWrapperGrid"))
            {
                _adWrapperGrid = adGrid;
                _adWrapperGrid.Loaded += adGrid_Loaded;
            }
            base.OnNavigatedTo(e);
        }

        void adGrid_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoadAd(ApplicationView.Value);
        }

        private void LoadAd(string id)
        {
            if (_adWrapperGrid == null)
                return;

            DestroyAd();
            _adController = new AdController(_adWrapperGrid, id);
            _adController.LoadAd();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_adWrapperGrid != null)
                _adWrapperGrid.Loaded -= adGrid_Loaded;

            DestroyAd();
            base.OnNavigatedFrom(e);
        }

        private void DestroyAd()
        {
            if (_adController != null)
                _adController.DestroyAd();
        }
    }
}