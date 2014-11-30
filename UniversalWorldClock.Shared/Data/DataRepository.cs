using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeZones;
using UniversalWorldClock.Domain;
using Windows.Storage;
using UniversalWorldClock.Views;

namespace UniversalWorldClock.Data
{
    public sealed class DataRepository : IDataRepository<CityInfo>
    {
        private static bool IsInitialized = false;
        private static readonly StorageFolder _folder = ApplicationData.Current.LocalFolder;
        private const string FILE_NAME = "clocks.dat";
        private static IEnumerable<CityInfo> _cities = Enumerable.Empty<CityInfo>();
        private static IEnumerable<CityInfo> _userCities = Enumerable.Empty<CityInfo>();

        public Task Save(IEnumerable<CityInfo> data)
        {
            return SaveInternal(data);
        }

        public IEnumerable<CityInfo> GetAll()
        {
            return _cities;
        }

        public IEnumerable<CityInfo> GetUsersCities()
        {
            return _userCities;
        }

        public IEnumerable<CityInfo> Get(Func<CityInfo, bool> predicate)
        {
            return _cities.Where(predicate);
        }

        public static async Task LoadCities()
        {
            if (!IsInitialized)
            {
                var uri = new System.Uri("ms-appx:///Assets/cities.csv");
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var buffer = await FileIO.ReadLinesAsync(file);
                _cities = buffer.Select(StringToCityInfo)
                    .Where(x => !string.IsNullOrEmpty(x.Name)).ToList();

                await LoadUserCities();
                IsInitialized = true;
            }
        }

        public static async Task LoadUserCities()
        {
            bool isOldFormat = false;
            var file = await OpenFileAsync();

            if (file == null)
                _userCities= Enumerable.Empty<CityInfo>();

            var data = await FileIO.ReadTextAsync(file);
            IEnumerable<CityInfo> clocks;

            try
            {
                clocks = JsonConvert.DeserializeObject<List<CityInfo>>(data) ?? Enumerable.Empty<CityInfo>();
            }
            catch (JsonSerializationException)
            {
                clocks = LoadOldFormatData(data);
                isOldFormat = true;
            }
            catch (JsonReaderException)
            {
                clocks = LoadOldFormatData(data);
                isOldFormat = true;
            }
            catch (Exception ex)
            {
                clocks = Enumerable.Empty<CityInfo>();
            }
            if (isOldFormat)
                await SaveInternal(clocks);
            _userCities = clocks ?? Enumerable.Empty<CityInfo>();
        }

        private static IEnumerable<CityInfo> LoadOldFormatData(string data)
        {
            try
            {
                return LoadOldFormatSafely(data);
            }
            catch (Exception)
            {
                return Enumerable.Empty<CityInfo>();
            }
        }

        private static IEnumerable<CityInfo> LoadOldFormatSafely(string data)
        {
            try
            {
                var ids = JsonConvert.DeserializeObject<List<int>>(data);
                var clocks = ids.Select(i => _cities.Single(c => c.Id == i));

                return clocks;
            }
            catch (JsonReaderException)
            {
                var oldData = JsonConvert.DeserializeObject<List<ClockInfo>>(data);

                return oldData.Select(i =>
                                      {
                                          var city =
                                              _cities.SingleOrDefault(
                                                  c =>
                                                      c.Name.Equals(i.CityName) && c.CountryName.Equals(i.CountryName) &&
                                                      c.CountryCode.Equals(i.CountryCode));
                                          return city;
                                      }).Where(i => i != null).ToList();
            }
            catch (Exception)
            {
                return Enumerable.Empty<CityInfo>();
            }
        }

        private static Task SaveInternal(IEnumerable<CityInfo> data)
        {
            return Task.Run(async () =>
            {
                var jsonData = JsonConvert.SerializeObject(data);
                var file = await CreateFileAsync();
                await FileIO.WriteTextAsync(file, jsonData);

            });
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

        private static async Task<StorageFile> CreateFileAsync()
        {
            return await _folder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
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
