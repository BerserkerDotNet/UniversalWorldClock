using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using UniversalWorldClock.Common;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Messages;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.Services;
using UniversalWorldClock.Tasks;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.ViewModels
{
    public class CityDetailsPageViewModel : ViewModel, ICityDetailsPageViewModel, IClockListner
    {
        private readonly IClock _clock;
        private readonly IDataRepository _repository;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private readonly ICacheClient _cacheClient;
        private ObservableCollection<CityInfo> _clocks;
        private CityInfo _info;
        private DateTime _date;

        public CityDetailsPageViewModel(IClock clock, IDataRepository repository, 
            IEventAggregator eventAggregator, 
            INavigationService navigationService,
            ICacheClient cacheClient)
        {
            _clock = clock;
            _repository = repository;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _cacheClient = cacheClient;
            var clocks = _repository.GetUsersCities();
            Clocks = new ObservableCollection<CityInfo>(clocks);
            ApplySelection = new DelegateCommand<SelectionChangedEventArgs>(ApplySelectionInternal);
            Delete = new RelayCommand(HandleDelete);
        }

        private void HandleDelete()
        {
            _eventAggregator.GetEvent<DeleteCityMessage>().Publish(_info);
            Clocks.Remove(_info);
            if (Clocks.Count == 0)
                _navigationService.Navigate(Experiences.Main.ToString(), null);
        }

        private void ApplySelectionInternal(SelectionChangedEventArgs obj)
        {
            if (obj.AddedItems.Count > 0)
            {
                var cityInfo = obj.AddedItems.First() as CityInfo;
                if (cityInfo != null)
                    _info = cityInfo;

                Date = _clock.ConvertTime(_info.TimeZoneId, DateTime.Now);
                OnPropertyChanged(() => UTCOffset);
            }
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            var clocks = _repository.GetUsersCities();
            Clocks = new ObservableCollection<CityInfo>(clocks);
            CityInfo = (CityInfo)navigationParameter;
            _clock.Register(this);
            Clocks.CollectionChanged += Clocks_CollectionChanged;
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            _clock.UnRegister(this);
            Clocks.CollectionChanged -= Clocks_CollectionChanged;
        }

        private void Clocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _repository.Save(Clocks);
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

        public ObservableCollection<CityInfo> Clocks
        {
            get { return _clocks; }
            set { SetProperty(ref _clocks, value); }
        }

        public ICommand ApplySelection { get; private set; }

        public ICommand Delete { get; private set; }

        public CityInfo CityInfo
        {
            get { return _info; }
            set { SetProperty(ref _info, value); }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                SetProperty(ref _date, value);
            }
        }

        public TimeSpan UTCOffset
        {
            get
            {
                var key = "offset_" + _info.TimeZoneId;
                var offset = _cacheClient.Get<TimeSpan>(key);
                if (offset == default(TimeSpan))
                {
                    offset = _clock.GetOffset(_info.TimeZoneId);
                    _cacheClient.Set(key, offset);
                }
                return offset;
            }
        }

        public ObservableCollection<SunInfo> SunInfo
        {
            get
            {
                var key = _info.Id + Date.ToString("d");
                var sunInfos = _cacheClient.Get<IEnumerable<SunInfo>>(key);
                if (sunInfos == null)
                {
                    sunInfos = GetSunInfo().ToList();
                    _cacheClient.Set(key, sunInfos);
                }
                return new ObservableCollection<SunInfo>(sunInfos);
            }
        }

        public string Coordinates
        {
            get
            {
                var latitude = GeoHelper.LatitudeFromDecimalDegrees(_info.Latitude);
                var longtitude = GeoHelper.LongtitudeFromDecimalDegrees(_info.Longitude);
                return string.Format("{0} {1}", latitude, longtitude);
            }
        }

        public void TickTack(DateTime date)
        {
            Date = date;
        }

        private IEnumerable<SunInfo> GetSunInfo()
        {
            for (int i = 0; i < 3; i++)
            {
                var dateTime = Date.AddDays(i);
                var sunInfo = GeoHelper.GetSunInfo(dateTime, _info.Latitude, -_info.Longitude);
                sunInfo.SunSet = _clock.ConvertTime(_info.TimeZoneId, sunInfo.SunSet);
                sunInfo.SunRise = _clock.ConvertTime(_info.TimeZoneId, sunInfo.SunRise);

                yield return sunInfo;
            }
        }
    }
}
