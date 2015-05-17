using Microsoft.Practices.Prism.Mvvm;
using UniversalWorldClock.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalWorldClock.Views
{
    public sealed partial class TimeMenu:IView
    {
        public TimeMenu()
        {
            this.InitializeComponent();
            DataContext = App.Resolve<TimeMenuViewModel>();
        }
    }


}
