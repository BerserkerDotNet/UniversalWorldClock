﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Runtime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalWorldClock.Views
{
    public sealed partial class CitiesSearchResultsPage : Common.LayoutAwarePage
    {
        private string _queryText;

        private IDataRepository<CityInfo> _citiesRepository;

        public CitiesSearchResultsPage()
        {
            this.InitializeComponent();
            _citiesRepository = DependencyResolver.Resolve<IDataRepository<CityInfo>>();
        }

        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            DefaultViewModel["IsInProgress"] = Visibility.Visible;
            _queryText = navigationParameter as String;

            var filteredCities = _citiesRepository.Get(c => c.Name.StartsWith(_queryText, StringComparison.OrdinalIgnoreCase)
                );

            var filteredCitiesByTimezone = _citiesRepository.Get(c => c.TimeZoneId.StartsWith(_queryText, StringComparison.OrdinalIgnoreCase));
            var filters = (from c in filteredCities
                           group c by c.CountryName
                               into g
                               select new { Name = g.Key, Count = g.Count(), Items = g.ToList() }).OrderByDescending(g => g.Count).Take(5).Select(
                               g => new Filter(g.Name, g.Count) { Cities = g.Items });

            var filtersByTimezone = (from c in filteredCitiesByTimezone
                                     group c by c.TimeZoneId
                                         into g
                                         select new { Name = g.Key, Count = g.Count(), Items = g.ToList() }).OrderByDescending(g => g.Count).Take(5).Select(
                               g => new Filter(g.Name, g.Count) { Cities = g.Items });

            var allFiltered = filteredCities.Union(filteredCitiesByTimezone).ToList();

            var filterList = new List<Filter>
                                 {
                                     new Filter("All", allFiltered.Count(), true) {Cities = allFiltered.ToList()}
                                 };

            filterList.AddRange(filters);
            filterList.AddRange(filtersByTimezone);



            // Communicate results through the view model
            this.DefaultViewModel["QueryText"] = '\u201c' + _queryText + '\u201d';
            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
            DefaultViewModel["IsInProgress"] = Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        async void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DefaultViewModel["IsInProgress"] = Visibility.Visible;
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
            if (selectedFilter != null)
            {
                selectedFilter.Active = true;
                var resultsData = selectedFilter.Cities.AsParallel()
                    .Select(r => new SearchResult
                    {
                        Id = r.Id,
                        Title = r.Name,
                        Subtitle = r.State,
                        Description = r.CountryName + " " + string.Format("{0}{1:00}:{2:00} UTC", r.CurrentOffset < TimeSpan.Zero ? string.Empty : "+", r.CurrentOffset.Hours, r.CurrentOffset.Minutes),
                        Image = new Uri(string.Format("ms-appx:///Assets/CountryFlags/{0}.png", r.CountryCode))
                    });

                DefaultViewModel["Results"] = resultsData.ToList();
              
                // Ensure results are found
                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    DefaultViewModel["IsInProgress"] = Visibility.Collapsed;
                    return;
                }
             
            }
            DefaultViewModel["IsInProgress"] = Visibility.Collapsed;
            // Display informational text when there are no search results.
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        /// <summary>
        /// Invoked when a filter is selected using a RadioButton when not snapped.
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
        /// <param name="e">Event data describing how the RadioButton was selected.</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        private sealed class Filter : Common.BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;

            public Filter(String name, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
                this.Active = active;
            }

            public override String ToString()
            {
                return Description;
            }

            public String Name
            {
                get { return _name; }
                set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged(); }
            }

            public int Count
            {
                get { return _count; }
                set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged(); }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }

            public List<CityInfo> Cities { get; set; }
        }

        private void OnResultSelected(object sender, ItemClickEventArgs e)
        {
            var id = (e.ClickedItem as SearchResult).Id;
            var result = _citiesRepository.Get(x => x.Id == id).Single();

            ViewModelStorage.Main.AddClock(result);

            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            frame.Navigate(typeof(MainPage));
            Window.Current.Content = frame;

            Window.Current.Activate();
        }
    }
}
