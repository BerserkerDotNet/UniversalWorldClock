using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using UniversalWorldClock.Common;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Messages;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.Services;

namespace UniversalWorldClock.ViewModels
{
    public class ClockViewModel : ViewModel, IClockListner
    {
        private CityInfo _info;
        private readonly IEventAggregator _eventAggregator;
        private readonly IClock _clock;

        private DateTime _date;

        public ClockViewModel(IEventAggregator eventAggregator, IClock clock)
        {
            _eventAggregator = eventAggregator;
            _clock = clock;
            Delete = new RelayCommand(DeleteItem);
            Initialize = new DelegateCommand<CityInfo>(InitializeInternal);
        }

        private void InitializeInternal(CityInfo cityInfo)
        {
            Info = cityInfo;
            _eventAggregator.GetEvent<BeforeHideClocksMessage>().Subscribe(OnBeforeHideClocks);
            _clock.Register(this);
        }

        public string CityName
        {
            get { return _info.Name; }
        }

        public CityInfo Info {
            get { return _info; }
            set { SetProperty(ref _info, value); }
        }

        public string CountryName
        {
            get { return _info.CountryName; }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                SetProperty(ref _date, value);
            }
        }

        public TimeSpan UTCOffset
        {
            get { return _clock.GetOffset(_info.TimeZoneId); }
        }

        public string CountryCode
        {
            get { return _info.CountryCode; }
        }

        public string Coordinates 
        {
            get
            {
                var latitude = GeoHelper.LatitudeFromDecimalDegrees(_info.Latitude);
                var longtitude = GeoHelper.LongtitudeFromDecimalDegrees(_info.Longitude);
                return string.Format("{0} {1}",latitude, longtitude);
            }
        }

        public string TimeZone {
            get { return _info.TimeZone; }
        }  

        public string TimeZoneId {
            get { return _info.TimeZoneId; }
        }

        public ObservableCollection<SunInfo> SunInfo
        {
            get
            {
                var info = GetSunInfo().ToList();
                return new ObservableCollection<SunInfo>(info);
            }
        }

        public bool IsTimeModifierVisible
        {
            get
            {
                return UCSettings.ClockFormat == ClockFormat.TwelveHourClock;
            }
        }

        public ICommand Delete { get; private set; }

        public ICommand Initialize { get; private set; }

        private void DeleteItem()
        {
            OnBeforeHideClocks(null);
            _eventAggregator.GetEvent<DeleteCityMessage>().Publish(_info);
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

        private void OnBeforeHideClocks(object obj)
        {
            _clock.UnRegister(this);
        }

        public CityInfo CityInfo
        {
            get { return _info; }
        }

        public void TickTack(DateTime date)
        {
            Date = date;
        }
    }

}