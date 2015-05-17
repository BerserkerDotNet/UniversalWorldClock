using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace UniversalWorldClock.Extensions
{
    public static class StorageExtensions
    {
        public static async Task<IStorageItem> TryGetItemAsync(this StorageFolder folder, string filename)
        {
            try
            {
                return  await folder.GetFileAsync(filename);
            }
            catch
            {
                return null;
            }
        }
    }
}
