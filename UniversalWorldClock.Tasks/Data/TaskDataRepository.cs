using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Newtonsoft.Json;
using UniversalWorldClock.Tasks.Domain;

namespace UniversalWorldClock.Tasks.Data
{
    public sealed class TaskDataRepository
    {
        private static readonly StorageFolder _folder = ApplicationData.Current.LocalFolder;
        private const string FILE_NAME = "Clocks.dat";
      
        public static IAsyncOperation<IEnumerable<CityInfo>> LoadUserCities()
        {
            return AsyncInfo.Run(async (t) =>
                                       {
                                           var file = await OpenFileAsync();

                                           if (file == null)
                                               return Enumerable.Empty<CityInfo>();

                                           var data = await FileIO.ReadTextAsync(file);
                                           IEnumerable<CityInfo> clocks;
                                           try
                                           {
                                               clocks = JsonConvert.DeserializeObject<List<CityInfo>>(data) ?? Enumerable.Empty<CityInfo>();
                                           }
                                           catch (Exception)
                                           {
                                               clocks = Enumerable.Empty<CityInfo>();
                                           }
                                           return clocks;
                                       });
        }

        private static async Task<StorageFile> OpenFileAsync()
        {
            return await _folder.CreateFileAsync(FILE_NAME, CreationCollisionOption.OpenIfExists);
        }
    }

    internal sealed class ClockInfo : IEquatable<ClockInfo>
    {
        public string CityName { get; set; }
        public string TimeZoneId { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public bool Equals(ClockInfo other)
        {
            if (ReferenceEquals(this, other))
                return true;

            return CityName.Equals(other.CityName) && TimeZoneId.Equals(other.TimeZoneId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClockInfo);
        }

        public override int GetHashCode()
        {
            return CityName.GetHashCode() ^ TimeZoneId.GetHashCode();
        }
    }
}
