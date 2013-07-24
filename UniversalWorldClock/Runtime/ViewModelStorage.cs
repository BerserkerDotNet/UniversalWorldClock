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
    }
}