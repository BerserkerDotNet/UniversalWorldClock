using System;
using System.Threading.Tasks;

namespace UniversalWorldClock.Domain
{
    public sealed class ClockInfo: IEquatable<ClockInfo>
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