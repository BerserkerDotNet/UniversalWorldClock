using System;
using System.Linq;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using Windows.Devices.Geolocation;

namespace UniversalWorldClock.ViewModels
{
    public sealed class CurrentLocationViewModel : ViewModelBase
    {
        private readonly IDataRepository<CityInfo> _citiesRepository;

        private CityInfo _locationCity;
        private ClockViewModel _currentClockViewModel;
        private readonly Geolocator _geolocator = new Geolocator();
        private PositionStatus _positionStatus;

        public CurrentLocationViewModel(IDataRepository<CityInfo> citiesRepository)
        {
            _citiesRepository = citiesRepository;
            InitializeLocation();
        }

        public CityInfo CurrentCity
        {
            get { return _locationCity; }
            private set
            {
                _locationCity = value;
                OnPropertyChanged();
            }
        }

        //NOTE: Smells bad!

        public ClockViewModel CurrentTime
        {
            get { return _currentClockViewModel; }
            set
            {
                _currentClockViewModel = value;
                OnPropertyChanged();
            }
        }

        public PositionStatus Status
        {
            get { return _positionStatus; }
            private set
            {
                _positionStatus = value;
                OnPropertyChanged();
            }
        }

        private async void InitializeLocation()
        {
            try
            {
                var pos = await _geolocator.GetGeopositionAsync();
                var cities = await _citiesRepository.Get();
                var city = cities.FirstOrDefault(c => Math.Abs(c.Latitude - pos.Coordinate.Latitude) < 0.03 && Math.Abs(c.Longitude - pos.Coordinate.Longitude) < 0.03);
                CurrentCity = city;
                var clockInfo = new ClockInfo
                                    {
                                        CityName = city.Name,
                                        CountryCode = city.CountryCode,
                                        CountryName = city.CountryName,
                                        TimeZoneId = city.TimeZoneId
                                    };
                CurrentTime = new ClockViewModel(clockInfo);
            }
            catch
            {
                
            }

            Status = _geolocator.LocationStatus;

        }
    }
}