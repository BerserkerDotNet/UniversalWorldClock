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
    public sealed partial class TimeShifter : UserControl
    {
        public TimeShifter()
        {
            this.InitializeComponent();
        }

        private void OnShifterLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = DependencyResolver.Resolve<TimeShifterViewModel>();
        }
    }
}
