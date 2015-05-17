using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.ViewModels.DesignTime
{
    public class DesignCitySelectorViewModel:ICitySelectorViewModel
    {
        public ObservableCollection<CityInfo> Cities
        {
            get
            {
                return new ObservableCollection<CityInfo>(new[]
                {
                    new CityInfo
                    {
                        Name = "Kiev",
                        CountryCode = "UA",
                        CountryName = "Ukraine",
                    }
                });
            }
            set {  }
        }

        public ICommand ApplyFilter
        {
            get { return new RelayCommand(() => { }); }
            set {}
        }

        public ICommand AddCity
        {
            get { return new RelayCommand(() => { }); }
            set {  }
        }

        public string Query
        {
            get { return "Kie"; }
            set {  }
        }
    }
}
