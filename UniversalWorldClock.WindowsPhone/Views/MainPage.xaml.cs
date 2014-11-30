using System;
using UniversalWorldClock.Common;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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


        protected override void OnSizeChanged(ApplicationView view)
        {
            if (CurrentViewState != ViewState.Snapped)
            {
                uiCurrentLocation.Margin = new Thickness(Window.Current.Bounds.Width - 1000, 0, 0, 0);
            }
            base.OnSizeChanged(view);
        }

        private void UiClockSelector_OnOpening(object sender, object e)
        {
            uiCitySelectorControl.ClearText();
        }

        private void CitySelector_OnCitySelected(object sender, EventArgs e)
        {
            uiClockSelector.Hide();
        }
    }
}
