using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalWorldClock.Common
{
    public class StateAwareUserControl : UserControl
    {
        public StateAwareUserControl()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            Loaded += StateAwareUserControl_Loaded;
        }

        void StateAwareUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetVisualState();
        }

        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            SetVisualState();
            OnSizeChanged(ApplicationView.Value);
        }

        private void SetVisualState()
        {
            var state = ApplicationView.Value.ToString();
            VisualStateManager.GoToState(this, state, false);
        }

        protected virtual void OnSizeChanged(ApplicationViewState viewState)
        {
            
        }
    }
}