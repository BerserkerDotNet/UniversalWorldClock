using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.ViewModels.DesignTime
{
    public class DesignMainPageViewModel:IMainPageViewModel
    {
        public void AddClock(CityInfo info)
        {
            
        }

        public void DeleteClock(CityInfo clock)
        {
        }

        public ICommand Add
        {
            get { return null; }
        }

        public ICommand SetTime
        {
            get { return null; }
        }

        public ObservableCollection<CityInfo> Clocks
        {
            get
            {
                return new ObservableCollection<CityInfo>(new[]
                {
                    new CityInfo
                    {
                        CountryCode = "UA",
                        CountryName = "Ukraine",
                        Latitude = 57.5f,
                        Longitude = 54.5f,
                        Name = "Kiev",
                        TimeZone = "UTC+02",
                        TimeZoneId = "UTC-Kiev",
                    }
                });
            }
            set {  }
        }

        public ICommand Donate
        {
            get { return null; }
        }

        public ICommand GoToDetails
        {
            get { return null; }
        }

        public bool MenuActive
        {
            get { return true; }
        }

        public ICommand	 SetShowMenu
        {
            get { return null; }
        }

        public ICommand GoToSettings
        {
            get { return null; }
        }

        public void Refresh()
        {
            
        }
    }
}