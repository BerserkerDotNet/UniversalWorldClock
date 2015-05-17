using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.ViewModels.DesignTime
{
    public class DesignCityDetailsPageViewModel : ICityDetailsPageViewModel
    {
        public CityInfo CityInfo
        {
            get { return new CityInfo
            {
                Name = "Kiev",
                CountryName = "Ukraine"
            }; }
            set { }
        }

        public DateTime Date {
            get
            {
                return DateTime.Now;
            }
            set { }
        }

        public TimeSpan UTCOffset {
            get { return TimeSpan.FromHours(2);}
        }

        public ObservableCollection<SunInfo> SunInfo
        {
            get
            {
                return new ObservableCollection<SunInfo>(new[]
                {
                    new SunInfo {SunRise = DateTime.Now, SunSet = DateTime.Now},
                    new SunInfo {SunRise = DateTime.Now, SunSet = DateTime.Now},
                    new SunInfo {SunRise = DateTime.Now, SunSet = DateTime.Now}
                });
            }
        }

        public string Coordinates
        {
            get { return "28.5245N 45.4578S"; }
        }

        public string TimeZoneId
        {
            get { return "Eastern European Summer Time"; }
        }

        public string TimeZone
        {
            get { return "Europe/Kiev"; }
        }

        public ObservableCollection<CityInfo> Clocks {
            get
            {
                return
                    new ObservableCollection<CityInfo>(new[]
                    {new CityInfo {Name = "Kiev", CountryName = "Ukraine"}, new CityInfo {Name = "Boston", CountryName = "United States"},});
            }
            set { }
        }

        public ICommand ApplySelection { get; private set; }

        public ICommand Delete { get; private set; }
    }
}