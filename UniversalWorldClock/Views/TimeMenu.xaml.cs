using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Practices.Prism.Mvvm;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalWorldClock.Views
{
    public sealed partial class TimeMenu:IView
    {
        public TimeMenu()
        {
            this.InitializeComponent();
        }
        private void FlipLoaded(object sender, RoutedEventArgs e)
        {
#if !WINDOWS_PHONE_APP
            var control = sender as FlipView;
            ButtonHide(control, "PreviousButtonHorizontal");
            ButtonHide(control, "NextButtonHorizontal");
            ButtonHide(control, "PreviousButtonVertical");
            ButtonHide(control, "NextButtonVertical");
#endif

        }

        private void ButtonHide(FlipView f, string name)
        {
            Button b;
            b = FindVisualChild<Button>(f, name);
            b.Opacity = 0.0;
            b.IsHitTestVisible = false;
        }

        private TChildItemType FindVisualChild<TChildItemType>(DependencyObject obj, string name) where TChildItemType : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is TChildItemType && ((FrameworkElement)child).Name == name)
                    return (TChildItemType)child;
                else
                {
                    var childOfChild = FindVisualChild<TChildItemType>(child, name);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }


}
