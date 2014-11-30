using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace UniversalWorldClock.Common
{
    public static class ViewStateHelper
    {
        public static ViewState GetViewState(ApplicationView view, Size windowSize)
        {
            var width = windowSize.Width;
            if (width <=320)
                return  ViewState.Snapped;
            else if (width > 320 && width <= 920)
                return ViewState.Narrow;
            else
                return ViewState.FullScreenLandscape;
        }
    }
}