// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using System;
using Windows.UI.Xaml.Input;
using UniversalWorldClock.Converters;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.ViewModels;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace UniversalWorldClock.Views
{
    public sealed partial class Clock
    {
        public Clock()
        {
            this.InitializeComponent();
        }
    }
}
