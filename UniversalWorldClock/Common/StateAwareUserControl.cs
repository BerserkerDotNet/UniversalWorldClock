using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalWorldClock.Common
{
    public abstract class StateAwareUserControl : UserControl
    {
        public StateAwareUserControl()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            Loaded += StateAwareUserControl_Loaded;
        }

        private void StateAwareUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var bounds = Window.Current.Bounds;
            SetVisualState(new Size(bounds.Width, bounds.Height));
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            SetVisualState(e.Size);
            OnSizeChanged();
        }

        private void SetVisualState(Size controlSize)
        {
            var state = ViewStateHelper.GetViewState(ApplicationView.GetForCurrentView(), controlSize);
            CurrentViewState = state;
            VisualStateManager.GoToState(this, state.ToString(), false);
        }

        public ViewState CurrentViewState
        {
            get;
            private set;
        }

        protected virtual void OnSizeChanged()
        {
        }
    }
}