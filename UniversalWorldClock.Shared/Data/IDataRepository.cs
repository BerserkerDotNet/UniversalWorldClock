using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.Data
{
    public interface IDataRepository<T>
    {
        Task Save(IEnumerable<T> data);
        IEnumerable<T> Get(Func<T, bool> predicate);
        IEnumerable<T> GetAll();
        IEnumerable<CityInfo> GetUsersCities();
    }
}