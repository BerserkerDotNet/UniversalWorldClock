using System;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.Common
{
    public static class GeoHelper
    {
        public static string LongtitudeFromDecimalDegrees(double degree)
        {
            return CalculateCoordinates(degree, d => d >= 0 ? "E" : "W");
        }

        public static string LatitudeFromDecimalDegrees(double degree)
        {
            return CalculateCoordinates(degree, d => d >= 0 ? "N" : "S");
        }

        public static SunInfo GetSunInfo(DateTime date,double latitude, double longtitude )
        {
            var a = (14 - date.Month) / 12;
            var y = date.Year + 4800 - a;
            var m = date.Month + (12 * a) - 3;

            var jdn = date.Day + ((153 * m + 2) / 5) + (365 * y) + (y / 4) - (y / 100) + (y / 400) - 32045;
            var jd = jdn + (((double)date.Hour - 12) / 24) + ((double)date.Minute / 1440) + ((double)date.Second / 86400);

            var mg = 2451545.0009;
            var longtitudeRadians = GetRadians(longtitude);
            var latitudeRadians = GetRadians(latitude);
            var fullCircle = GetRadians(360);
            var nStar = (jd - mg) - (longtitudeRadians / fullCircle);
            var n = Math.Round(nStar);

            var solarNoonApprox = mg + (longtitudeRadians / fullCircle) + n;

            var meanAnomaly = (357.5291 + 0.98560028 * (solarNoonApprox - 2451545)) % 360;
            var meanRad = GetRadians(meanAnomaly);
            var equationCenter = (1.9148 * Math.Sin(meanRad)) + (0.0200 * Math.Sin(2 * meanRad)) + (0.0003 * Math.Sin(3 * meanRad));
            var eclipticLongitude = GetRadians((meanAnomaly + 102.9372 + equationCenter + 180) % 360);

            var solarTransit = solarNoonApprox + (0.0053 * Math.Sin(meanRad)) - (0.0069 * Math.Sin(2 * eclipticLongitude));
            var sunDeclination = Math.Asin(Math.Sin(eclipticLongitude) * Math.Sin(0.409279710));
            var hourAngle = Math.Acos((Math.Sin(-0.011448623) - Math.Sin(latitudeRadians) * Math.Sin(sunDeclination)) /
                            (Math.Cos(latitudeRadians) * Math.Cos(sunDeclination)));
            var sunSet = mg + ((hourAngle + longtitudeRadians) / fullCircle) + n + 0.0053 * Math.Sin(meanRad) -
                         0.0069 * Math.Sin(2 * eclipticLongitude);
             var sunRise = solarTransit - (sunSet - solarTransit);


            var sunSetTime = new DateTime(DoubleDateToTicks(sunSet - 2415018.5),DateTimeKind.Unspecified).ToLocalTime();
            var sunRiseTime = new DateTime(DoubleDateToTicks(sunRise - 2415018.5),DateTimeKind.Unspecified).ToLocalTime();

            return new SunInfo
                   {
                       SunSet = sunSetTime,
                       SunRise = sunRiseTime
                   };
        }

        private static long DoubleDateToTicks(double value)
        {
            if (value >= 2958466.0 || value <= -657435.0)
                throw new ArgumentException("value");
            var num1 = (long)(value * 86400000.0 + (value >= 0.0 ? 0.5 : -0.5));
            if (num1 < 0L)
                num1 -= num1 % 86400000L * 2L;
            var num2 = num1 + 59926435200000L;
            if (num2 < 0L || num2 >= 315537897600000L)
                throw new ArgumentException("value");
            else
                return num2 * 10000L;
        }
        
        private static double GetRadians(double angle)
        {
            return angle * Math.PI / 180;
        }

        private static string CalculateCoordinates(double degree, Func<int, string> letterEq)
        {
            var d = Math.Truncate(degree);
            var m = Math.Truncate((degree - d)*60);
            var s = ((degree - d)*3600) - (m*60);

            return string.Format("{0}°{1}'{2}\"{3}", Math.Abs(d), Math.Abs(m), Math.Abs(Math.Round(s,1)), letterEq((int)d));
        }

    }
 }