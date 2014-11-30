using System;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using System.ComponentModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalWorldClock.Controls
{
    public sealed partial class TimeSetter  
    {

        public TimeSetter()
        {
            this.InitializeComponent();
            DataContext = DependencyResolver.Resolve<TimeSetterViewModel>();
        }

    }
}
