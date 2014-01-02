using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UniversalWorldClock.Common;
using UniversalWorldClock.Runtime;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalWorldClock.ViewModels
{
    public sealed class SettingsViewModel : ViewModelBase
    {
        private string _clockSize;
        private string _clockFormat;

        public SettingsViewModel()
        {

            LoadSettings();
            Save = new RelayCommand(ExecuteSave);
        }

        private void LoadSettings()
        {
            ClockSize = UCSettings.ClockSize ?? ClockSizeList.First();
            ClockFormat = UCSettings.ClockFormat== Common.ClockFormat.TwelveHourClock ? "12h" : "24h";
        }

        public string ClockSize
        {
            get { return _clockSize; }
            set
            {
                if(_clockSize!=value)
                {
                    _clockSize = value;
                    OnPropertyChanged();
                }
            }
        } 
        public string ClockFormat
        {
            get { return _clockFormat; }
            set
            {
                if(_clockFormat!=value)
                {
                    _clockFormat = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand Save { get; set; }
        private void ExecuteSave()
        {
            UCSettings.ClockSize = ClockSize;
            UCSettings.ClockFormat = ClockFormat =="12h"? Common.ClockFormat.TwelveHourClock:Common.ClockFormat.TwentyFourClock;
            var f =(Window.Current.Content as Frame);
            f.Navigate(f.Content.GetType());
            f.GoBack();
            SettingsPane.Show();
        }

        public IList<string> ClockSizeList
        {
            get { return new List<string> {"Small", "Medium", "Large"}; }
        }  
        public IList<string> ClockFormatList
        {
            get { return new List<string> {"12h", "24h"}; }
        }
    }
}