using System;
using TimeZones;

namespace UniversalWorldClock.Domain
{
    public sealed class CityInfo
    {
        private TimeSpan? _currentOffset;

        public int Id { get; set; }
        public string Name { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string CountryCode { get; set; }
        public int Population { get; set; }
        public int Elevation { get; set; }
        public string TimeZone { get; set; }
        public string TimeZoneId { get; set; }
        public string CountryName { get; set; }
        public string State { get; set; }
        public TimeSpan CurrentOffset {
            get
            {
                if (!_currentOffset.HasValue)
                {
                    var timeZoneService = TimeZoneService.FindSystemTimeZoneById(TimeZoneId);
                    var dateTimeOffset = timeZoneService.ConvertTime(DateTime.Now);
                    _currentOffset = dateTimeOffset.Offset;
                }
                return _currentOffset.Value;
            }
        }
    }
}
