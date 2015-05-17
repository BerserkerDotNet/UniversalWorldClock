using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Microsoft.Practices.Prism.Mvvm;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Services;

namespace UniversalWorldClock.ViewModels
{
    public sealed class CurrentLocationViewModel : ViewModel, IClockListner
    {
        private readonly IDataRepository _citiesRepository;
        private readonly IClock _clock;

        private CityInfo _locationCity;
        private readonly Geolocator _geolocator = new Geolocator();
        private PositionStatus _positionStatus;
        private bool _isLoading;
        private DateTime _date;

        public CurrentLocationViewModel(IDataRepository citiesRepository, IClock clock)
        {
            _citiesRepository = citiesRepository;
            _clock = clock;
            InitializeLocation();
        }

        public CityInfo CurrentCity
        {
            get { return _locationCity; }
            private set { SetProperty(ref _locationCity, value); }
        }

        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }
        
        public TimeSpan UTCOffset
        {
            get
            {
                if (_locationCity == null)
                    return TimeSpan.Zero;

                return _clock.GetOffset(_locationCity.TimeZoneId);
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        public PositionStatus Status
        {
            get { return _positionStatus; }
            private set { SetProperty(ref _positionStatus, value); }
        }

        private async void InitializeLocation()
        {
            try
            {
                IsLoading = true;
                var pos = await _geolocator.GetGeopositionAsync();
                var city = _citiesRepository.Get(c => Math.Abs(c.Latitude - pos.Coordinate.Latitude) < 0.03 && Math.Abs(c.Longitude - pos.Coordinate.Longitude) < 0.03).FirstOrDefault();
                CurrentCity = city;
                Status = _geolocator.LocationStatus;
                _clock.Register(this);
                OnPropertyChanged(()=>UTCOffset);
            }
            catch
            {
                Status = PositionStatus.NotAvailable;
            }
            finally
            {
                IsLoading = false;
            }

        }

        public CityInfo CityInfo
        {
            get { return _locationCity; }
        }

        public void TickTack(DateTime date)
        {
            Date = date;
        }
    }
}