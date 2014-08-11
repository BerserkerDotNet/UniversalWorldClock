using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UniversalWorldClock.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            SignalPropertyChange(propertyName);
        }

        private void SignalPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
           
        }

        protected void ManualPropertyChanged(string propertyName = null)
        {
            SignalPropertyChange(propertyName);
        }
    }
}