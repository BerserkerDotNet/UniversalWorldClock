using System;
using LeadBolt.Windows8.AppAd;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UniversalWorldClock.Common
{
    public class AdAwarePage : LayoutAwarePage
    {
        private AdController _adController;
        private Grid _adWrapperGrid;
        private static DispatcherTimer _adTimer = new DispatcherTimer();


        static AdAwarePage()
        {
            _adTimer.Interval = TimeSpan.FromMinutes(15);
            _adTimer.Start();
        }

        protected override void OnSizeChanged(ApplicationViewState viewState)
        {
            LoadAd(viewState);
        }

        private void LoadAd(ApplicationViewState viewState)
        {
            if (viewState == ApplicationViewState.Snapped)
                LoadAd();
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
                _adTimer.Tick += _adTimer_Tick;
            }
            base.OnNavigatedTo(e);
        }

        void _adTimer_Tick(object sender, object e)
        {
            if (ApplicationView.Value == ApplicationViewState.Snapped)
                LoadAd();
        }

        void adGrid_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoadAd(ApplicationView.Value);
        }

        private void LoadAd()
        {
            if (_adWrapperGrid == null)
                return;

            DestroyAd();
            _adController = new AdController(_adWrapperGrid, "198903296");
            _adController.LoadAd();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_adWrapperGrid != null)
                _adWrapperGrid.Loaded -= adGrid_Loaded;
            _adTimer.Tick -= _adTimer_Tick;
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