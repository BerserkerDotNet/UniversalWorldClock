using System;
using System.Linq;
using System.Threading.Tasks;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Runtime;
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
        private bool _isLoading;

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

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
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
                IsLoading = true;
                var pos = await _geolocator.GetGeopositionAsync();
                var city = _citiesRepository.Get(c => Math.Abs(c.Latitude - pos.Coordinate.Latitude) < 0.03 && Math.Abs(c.Longitude - pos.Coordinate.Longitude) < 0.03).FirstOrDefault();
                CurrentCity = city;
                CurrentTime = DependencyResolver.Resolve<ClockViewModel>(new Tuple<string, object>("info", city));
                Status = _geolocator.LocationStatus;
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
    }
}