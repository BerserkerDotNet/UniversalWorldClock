using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZones;
using UniversalWorldClock.Domain;
using Windows.Storage;

namespace UniversalWorldClock.Data
{
    public sealed class CitiesRepository : IDataRepository<CityInfo>
    {
        public Task Save(IEnumerable<CityInfo> data)
        {
            throw new NotSupportedException();
        }

        public async Task<IEnumerable<CityInfo>> Get()
        {
            var uri = new System.Uri("ms-appx:///Assets/cities.csv");
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

            var buffer = await FileIO.ReadLinesAsync(file);
            var data = buffer.Select(StringToCityInfo)
                .Where(x => !string.IsNullOrEmpty(x.Name)).ToList();


            return data;
        }

        private static CityInfo StringToCityInfo(string c)
        {
            var info = c.Split(new[] {';'});
            return new CityInfo
                       {
                           Id = int.Parse(info[0]),
                           Name = info[1],
                           Latitude = float.Parse(info[2]),
                           Longitude = float.Parse(info[3]),
                           CountryCode = info[4],
                           Population = int.Parse(info[5]),
                           Elevation = int.Parse(info[6]),
                           TimeZone = info[7],
                           TimeZoneId = info[8],
                           CountryName = info[9],
                           State = info[10],
                       };
        }


    }
}
