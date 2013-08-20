using System;
using System.Linq;
using Microsoft.Practices.Unity;
using UniversalWorldClock.Common;
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
            _container.RegisterType<TimeShiftProvider>(new ContainerControlledLifetimeManager());
        }

        public static T Resolve<T>(params Tuple<string, object>[] arguments)
        {
            var overrides = arguments.Select(t => new ParameterOverride(t.Item1, t.Item2))
                .Cast<ResolverOverride>()
                .ToArray();
            return _container.Resolve<T>(overrides);
        }
    }
}