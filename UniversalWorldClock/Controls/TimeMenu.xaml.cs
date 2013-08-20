using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalWorldClock.Controls
{
    public sealed partial class TimeMenu
    {
        public TimeMenu()
        {
            this.InitializeComponent();
            DataContext = DependencyResolver.Resolve<TimeMenuViewModel>();
        }
        private void FlipLoaded(object sender, RoutedEventArgs e)
        {
            var control = sender as FlipView;

            ButtonHide(control, "PreviousButtonHorizontal");
            ButtonHide(control, "NextButtonHorizontal");
            ButtonHide(control, "PreviousButtonVertical");
            ButtonHide(control, "NextButtonVertical");

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
