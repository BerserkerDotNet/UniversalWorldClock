using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversalWorldClock.Data
{
    public interface IDataRepository<T>
    {
        Task Save(IEnumerable<T> data);
        Task<IEnumerable<T>> Get();
    }
}