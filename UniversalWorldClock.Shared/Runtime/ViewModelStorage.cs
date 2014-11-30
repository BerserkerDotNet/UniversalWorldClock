using UniversalWorldClock.ViewModels;

namespace UniversalWorldClock.Runtime
{
    public sealed class ViewModelStorage
    {
        private static MainViewModel _main;
        public static MainViewModel Main
        {
            get { return _main ?? (_main = DependencyResolver.Resolve<MainViewModel>()); }
        }
        private static CurrentLocationViewModel _currentLocation;
        public static CurrentLocationViewModel CurrentLocation
        {
            get { return _currentLocation ?? (_currentLocation = DependencyResolver.Resolve<CurrentLocationViewModel>()); }
        }
    }
}