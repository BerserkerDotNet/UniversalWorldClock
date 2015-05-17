using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using UniversalWorldClock.Common;
using UniversalWorldClock.Runtime;

namespace UniversalWorldClock.ViewModels
{
    public sealed class OptionsViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private string _clockSize;
        private string _clockFormat;

        public OptionsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

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
                SetProperty(ref _clockSize, value);
            }
        } 
        public string ClockFormat
        {
            get { return _clockFormat; }
            set
            {
                SetProperty(ref _clockFormat, value);
            }
        }

        public ICommand Save { get; set; }
        private void ExecuteSave()
        {
            UCSettings.ClockSize = ClockSize;
            UCSettings.ClockFormat = ClockFormat == "12h"
                ? Common.ClockFormat.TwelveHourClock
                : Common.ClockFormat.TwentyFourClock;
            var f = (Window.Current.Content as Frame);
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