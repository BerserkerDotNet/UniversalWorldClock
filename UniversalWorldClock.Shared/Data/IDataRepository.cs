using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.Data
{
    public interface IDataRepository
    {
        Task Save(IEnumerable<CityInfo> data);
        IEnumerable<CityInfo> Get(Func<CityInfo, bool> predicate);
        IEnumerable<CityInfo> GetAll();
        IEnumerable<CityInfo> GetUsersCities();
    }
}