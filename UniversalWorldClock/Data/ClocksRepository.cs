using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels;
using Windows.Storage;

namespace UniversalWorldClock.Data
{
    public sealed class ClocksRepository: IDataRepository<ClockInfo>
    {
        private readonly StorageFolder _folder = ApplicationData.Current.LocalFolder;
        private const string FILE_NAME = "clocks.dat";

        public Task Save(IEnumerable<ClockInfo> data)
        {
            return Task.Run(async () =>
                                      {
                                          var jsonData = JsonConvert.SerializeObject(data);
                                          var file = await CreateFileAsync();
                                          await FileIO.WriteTextAsync(file, jsonData);
                                      });
        }

        public async Task<IEnumerable<ClockInfo>> Get()
        {
            var file = await OpenFileAsync();

            if (file == null)
                return Enumerable.Empty<ClockInfo>();

            var data = await FileIO.ReadTextAsync(file);
            var clocks = JsonConvert.DeserializeObject<List<ClockInfo>>(data);

            return clocks ?? Enumerable.Empty<ClockInfo>();
        }

        private async Task<StorageFile> CreateFileAsync()
        {
            return await _folder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
        } 
        private async Task<StorageFile> OpenFileAsync()
        {
            return await _folder.CreateFileAsync(FILE_NAME, CreationCollisionOption.OpenIfExists);
        }
    }
}