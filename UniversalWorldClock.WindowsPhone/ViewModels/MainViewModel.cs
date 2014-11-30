using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.Tasks;
using UniversalWorldClock.Views;


namespace UniversalWorldClock.ViewModels
{
    public sealed class MainViewModel :  ViewModelBase
    {
        #region Fields
        private ObservableCollection<CityInfo> _clocks;
        private IDataRepository<CityInfo> _citiesRepository;

        private List<CityInfo> _cities; 

        #endregion

        #region Public Methods
        public MainViewModel(IDataRepository<CityInfo> citiesRepository)
        {
            _citiesRepository = citiesRepository;
            _cities = _citiesRepository.GetAll().ToList();
            Initialize();
        }

        public void AddClock(CityInfo info)
        {
            if (!Clocks.Contains(info))
                Clocks.Add(info);
        }
        public void DeleteClock(CityInfo clock)
        {
            if (!Clocks.Contains(clock))
                return;

            Clocks.Remove(clock);

            //NOTE: Need a way to cleanup 
        } 

        #endregion

        #region Public Properties

        public ICommand Add { get; private set; }
        public ICommand SetTime { get; private set; }

        public IEnumerable<CityInfo> Countries {
            get { return _cities; }
        }

        public void FilterCountries(string query)
        {
            _cities = _citiesRepository.GetAll().Where(c => c.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)).ToList();
            ManualPropertyChanged("Countries");
        }

        public ObservableCollection<CityInfo> Clocks
        {
            get { return _clocks; }
            set
            {
                if (_clocks != value)
                {
                    _clocks = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand Donate { get; private set; }

        #endregion


        private void Initialize()
        {
            Add = new RelayCommand(() => { (Window.Current.Content as Frame).Navigate(typeof(SearchCity)); });
            Donate = new RelayCommand(() => Launcher.LaunchUriAsync(new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=UFS2JX3EJGU3N")));

            var clocks = _citiesRepository.GetUsersCities();
            Clocks = new ObservableCollection<CityInfo>(clocks);
            Clocks.CollectionChanged += Clocks_CollectionChanged;
        }
        private void Clocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _citiesRepository.Save(Clocks);
            var updater = new LiveTileScheduler();

            Task.Factory.StartNew(() =>
            {
                updater.ReSchedule(Clocks.Select(c => new Tasks.Domain.CityInfo
                {
                    Id = c.Id,
                    Name = c.Name,
                    TimeZoneId = c.TimeZoneId
                }));
            }, TaskCreationOptions.LongRunning);

        }

        public void Refresh()
        {
            ManualPropertyChanged("Clocks");
        }
    }
}
