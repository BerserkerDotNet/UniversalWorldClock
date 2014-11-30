using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;
using TimeZones;
using UniversalWorldClock.Tasks.Data;
using UniversalWorldClock.Tasks.Domain;

namespace UniversalWorldClock.Tasks
{
    public sealed class LiveTileScheduler
    {
        #region Constants
        const string TileUpdateMessageFor3OrMoreItems = @"<tile><visual version=""2"">
                                        <binding template=""TileSquare150x150Text02"" fallback=""TileSquareText02""><text id=""1"">{0}</text><text id=""2"">{1}</text></binding>
                                        <binding template=""TileWide310x150SmallImageAndText04"" fallback=""TileWideSmallImageAndText04"">      
                                              <image id=""1"" src=""ms-appx:///assets/CountryFlagsTile/{2}.png""/>
                                              <text id=""1"">{0}</text>
                                              <text id=""2"">{1}</text>
                                         </binding>
                                        <binding template=""TileSquare310x310SmallImagesAndTextList03"">
                                          <image id=""1"" src=""ms-appx:///assets/CountryFlagsTile/{2}.png"" />
                                          <image id=""2"" src=""ms-appx:///assets/CountryFlagsTile/{7}.png"" />
                                          <image id=""3"" src=""ms-appx:///assets/CountryFlagsTile/{8}.png"" />
                                          <text id=""1"">{0}</text>
                                          <text id=""2"">{1}</text>
                                          <text id=""3"">{3}</text>
                                          <text id=""4"">{4}</text>
                                          <text id=""5"">{5}</text>
                                          <text id=""6"">{6}</text>
                                        </binding>
                                   </visual></tile>";

        const string TileUpdateMessageForLessThen3Items = @"<tile><visual version=""2"">
                                        <binding template=""TileSquare150x150Text02"" fallback=""TileSquareText02""><text id=""1"">{0}</text><text id=""2"">{1}</text></binding>
                                        <binding template=""TileWide310x150SmallImageAndText04"" fallback=""TileWideSmallImageAndText04"">      
                                              <image id=""1"" src=""ms-appx:///assets/CountryFlagsTile/{2}.png""/>
                                              <text id=""1"">{0}</text>
                                              <text id=""2"">{1}</text>
                                         </binding>
                                  <binding template=""TileSquare310x310SmallImageAndText01"">
                                      <image id=""1"" src=""ms-appx:///assets/CountryFlagsTile/{2}.png"" />
                                      <text id=""1"">{0}</text>
                                      <text id=""2"">{1}</text>
                                      <text id=""3""></text>
                                    </binding>
                                   </visual></tile>"; 
        #endregion

        private readonly Random _random = new Random();
        private static object _locker = new object();
        private Dictionary<CityInfo, DateTime> _baseDateTimes;

        public IAsyncAction ReSchedule(IEnumerable<CityInfo> userCities)
        {
            return AsyncInfo.Run(async t =>
                                       {
                                           lock (_locker)
                                           {
                                               Schedule(userCities, true);
                                           }

                                       });
        }

        public IAsyncAction CreateSchedule()
        {
            return AsyncInfo.Run(async t =>
            {
                var watch = new Stopwatch();
                watch.Start();
                var userCities = await TaskDataRepository.LoadUserCities();
                lock (_locker)
                {
                    Schedule(userCities, false);
                }
            });
        }

        private void Schedule(IEnumerable<CityInfo> userCities, bool reschedule)
        {
            if (!userCities.Any())
                return;

            var clockFormat = TaskSettings.GetClockFormatOrDefault();

            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            if (reschedule)
            {
                ClearQueue(tileUpdater);
            }
            var plannedUpdated = tileUpdater.GetScheduledTileNotifications();
            var now = DateTime.Now;

            var baseTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Local).AddMinutes(1);
            if (plannedUpdated.Count > 0)
                baseTime = plannedUpdated.Select(x => x.DeliveryTime.DateTime).Union(new[] { baseTime }).Max();
            var sw = new Stopwatch();
            sw.Start();
            CalculateBaseTime(userCities, baseTime);

            for (var i=0; i<=20;i++)
            {
                try
                {
                    ScheduleTileUpdate(baseTime, TimeSpan.FromMinutes(i), tileUpdater, clockFormat);
                    //Debug.WriteLine("SCHEDULED FOR: {0}; Elapsed: {1}", baseTime.AddMinutes(i), sw.Elapsed);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("exception: " + e.Message);
                }
            }
            sw.Stop();
            Debug.WriteLine("Total time: {0}", sw.Elapsed);
        }

        private void CalculateBaseTime(IEnumerable<CityInfo> userCities, DateTime baseTime)
        {
            _baseDateTimes = userCities.ToDictionary(u => u, u => GetUserCityTime(u, baseTime));
        }

        private void ClearQueue(TileUpdater tileUpdater)
        {
            foreach (var update in tileUpdater.GetScheduledTileNotifications())
            {
                tileUpdater.RemoveFromSchedule(update);
            }
        }

        private void ScheduleTileUpdate(DateTime baseTime, TimeSpan shift, TileUpdater tileUpdater, ClockFormat clockFormat)
        {
            var shiftedTime = baseTime.Add(shift);
            var interval = 60/_baseDateTimes.Count();
            var i = 0;
            var calculatedCitiesInfo = _baseDateTimes.Select(c => new CalculatedCityInfo { CityInfo = c.Key, DestinationTime = c.Value.Add(shift) }).ToList();
            foreach (var info in calculatedCitiesInfo)
            {
                var tileXmlNow = GetTileUpdateMessage(info, calculatedCitiesInfo, _baseDateTimes.Count(), clockFormat);
                var document = new XmlDocument();
                document.LoadXml(tileXmlNow);

                var deliveryTime = shiftedTime.AddSeconds(i * interval);
                if (deliveryTime <= DateTime.Now)
                {
                    tileUpdater.Update(new TileNotification(document) { ExpirationTime = shiftedTime.AddSeconds(interval) });
                }
                else
                {
                    var scheduledNotification = new ScheduledTileNotification(document, deliveryTime) { ExpirationTime = deliveryTime.AddSeconds(interval) };
                    tileUpdater.AddToSchedule(scheduledNotification);
                }
                i++;
            }
        }

        private string GetTileUpdateMessage(CalculatedCityInfo info, IEnumerable<CalculatedCityInfo> calculatedCitiesInfo, int userCititesCount, ClockFormat clockFormat)
        {
            var format = clockFormat == ClockFormat.TwentyFourClock ? "HH:mm" : "hh:mm tt";
            var cityInfo = info.CityInfo;
            var cityDescription = string.Format("{0}, {1}", cityInfo.Name, info.DestinationTime.ToString("M"));
            var largeTileCandidates = calculatedCitiesInfo
                .Where(c => !c.CityInfo.Equals(cityInfo));
            if (userCititesCount < 3)
                return string.Format(TileUpdateMessageForLessThen3Items, info.DestinationTime.ToString(format),
                    cityDescription,
                    cityInfo.CountryCode);


            var cityIndex1 = GetRandom(0, largeTileCandidates.Count(), -1);
            var cityIndex2 = GetRandom(0, largeTileCandidates.Count(), cityIndex1);
            var largeCity2 = largeTileCandidates.ElementAt(cityIndex1);
            var largeCity3 = largeTileCandidates.ElementAt(cityIndex2);
            var city2Description = string.Format("{0}, {1}", largeCity2.CityInfo.Name, largeCity2.DestinationTime.ToString("M"));
            var city3Description = string.Format("{0}, {1}", largeCity3.CityInfo.Name, largeCity3.DestinationTime.ToString("M"));
            return string.Format(TileUpdateMessageFor3OrMoreItems, info.DestinationTime.ToString(format), cityDescription,
                cityInfo.CountryCode,
                largeCity2.DestinationTime.ToString(format), city2Description,
                largeCity3.DestinationTime.ToString(format), city3Description,
                largeCity2.CityInfo.CountryCode, largeCity3.CityInfo.CountryCode);
        }

        private DateTime GetUserCityTime(CityInfo info, DateTime time)
        {
            var service = TimeZoneService.FindSystemTimeZoneById(info.TimeZoneId);
            var userCityTime = service.ConvertTime(time);
            return userCityTime.DateTime;
        }

        private int GetRandom(int min, int max, int exclude)
        {
            while (true)
            {
                var n = _random.Next(min, max);
                if (n!=exclude)
                {
                    return n;
                }
            }
            

        }

        private class CalculatedCityInfo
        {
            public CityInfo CityInfo { get; set; }
            public DateTime DestinationTime { get; set; }
        }

    }
}