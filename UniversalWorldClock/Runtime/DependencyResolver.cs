using Microsoft.Practices.Unity;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.Runtime
{
    public static class DependencyResolver
    {
        private static UnityContainer _container = new UnityContainer();
        static DependencyResolver()
        {
            InitMappings();
        }

        private static void InitMappings()
        {
            _container.RegisterType<IDataRepository<CityInfo>, CitiesRepository>();
            _container.RegisterType<IDataRepository<ClockInfo>, ClocksRepository>();
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}