using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Messages;
using UniversalWorldClock.Models;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.ViewModels
{

    public class CitiesSearchResultsPageViewModel : ViewModel, ICitiesSearchResultsPageViewModel
    {
        private readonly IDataRepository _repository;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private ObservableCollection<SearchFilter> _filters;
        private ObservableCollection<SearchResult> _results;
        private bool _showFilters;
        private bool _isInProgress;
        private string _queryText;
        private SearchFilter _currentFilter;

        public CitiesSearchResultsPageViewModel(IDataRepository repository, IEventAggregator eventAggregator, INavigationService navigationService)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;

            ApplyFilter = new RelayCommand(() => ApplyCurrentFilter(Filters.SingleOrDefault(f => f.Active)));
            SelectFilter = new DelegateCommand<SelectionChangedEventArgs>(GetAndApplyFilter);
            AddCity = new DelegateCommand<ItemClickEventArgs>(AddCityInternal);
        }



        public ObservableCollection<SearchFilter> Filters
        {
            get { return _filters; }
            set
            {
                if (_filters != value)
                {
                    _filters = value;
                    OnPropertyChanged(() => Filters);
                }
            }
        }
        public ObservableCollection<SearchResult> Results
        {
            get { return _results; }
            set
            {
                if (_results != value)
                {
                    _results = value;
                    OnPropertyChanged(() => Results);
                }
            }
        }
        public bool ShowFilters
        {
            get { return _showFilters; }
            set
            {
                _showFilters = value;
                OnPropertyChanged(() => ShowFilters);
            }
        }
        public bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                _isInProgress = value;
                OnPropertyChanged(() => IsInProgress);
            }
        }
        public string QueryText
        {
            get { return _queryText; }
            set
            {
                _queryText = value;
                OnPropertyChanged(() => QueryText);
            }
        }

        public SearchFilter CurrentFilter
        {
            get { return _currentFilter; }
            set { SetProperty(ref _currentFilter, value); }
        }

        public ICommand ApplyFilter { get; set; }

        public ICommand SelectFilter { get; set; }

        public ICommand AddCity { get; set; }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            IsInProgress = true;
            QueryText = navigationParameter as String;

            var filteredCities = _repository.Get(c => c.Name.StartsWith(_queryText, StringComparison.OrdinalIgnoreCase));
            var filteredCitiesByTimezone = _repository.Get(c => c.TimeZoneId.StartsWith(_queryText, StringComparison.OrdinalIgnoreCase));
            var filters = GetFiltersByCity(filteredCities);
            var filtersByTimezone = GetFiltersByTimeZone(filteredCitiesByTimezone);

            var allFiltered = filteredCities.Union(filteredCitiesByTimezone).ToList();

            var filterList = new List<SearchFilter>
            {
                new SearchFilter("All", allFiltered.Count(), true) {Cities = allFiltered.ToList()}
            };

            filterList.AddRange(filters);
            filterList.AddRange(filtersByTimezone);
            Filters = new ObservableCollection<SearchFilter>(filterList);
            CurrentFilter = filterList.FirstOrDefault();
            ShowFilters= filterList.Count > 1;
            IsInProgress = false;
        }

        private static IEnumerable<SearchFilter> GetFiltersByTimeZone(IEnumerable<CityInfo> filteredCitiesByTimezone)
        {
            return (from c in filteredCitiesByTimezone
                group c by c.TimeZoneId
                into g
                select new {Name = g.Key, Count = g.Count(), Items = g.ToList()}).OrderByDescending(g => g.Count)
                .Take(5)
                .Select(g => new SearchFilter(g.Name, g.Count) {Cities = g.Items});
        }

        private static IEnumerable<SearchFilter> GetFiltersByCity(IEnumerable<CityInfo> filteredCities)
        {
            return (from c in filteredCities
                group c by c.CountryName
                into g
                select new {Name = g.Key, Count = g.Count(), Items = g.ToList()}).OrderByDescending(g => g.Count)
                .Take(5)
                .Select(g => new SearchFilter(g.Name, g.Count) {Cities = g.Items});
        }

        private void GetAndApplyFilter(SelectionChangedEventArgs e)
        {
            var selectedFilter = e.AddedItems.FirstOrDefault() as SearchFilter;
            if (selectedFilter != null)
            {
                Filters.ForEach(f =>
                {
                    if (f.Name != selectedFilter.Name)
                        f.Active = false;
                });
                selectedFilter.Active = true;
            }
                
           // ApplyCurrentFilter(selectedFilter);
        }

        private void ApplyCurrentFilter(SearchFilter filter)
        {
            if(filter==null)
                return;

            IsInProgress = true;
            var resultsData = filter.Cities.AsParallel()
                .Select(ConvertCityToSearchResult).ToList();

            Results = new ObservableCollection<SearchResult>(resultsData);
            IsInProgress = false;

        }

        private void AddCityInternal(ItemClickEventArgs e)
        {
            var searchResult = (e.ClickedItem as SearchResult);
            if (searchResult == null)
                return;

            var id = searchResult.Id;
            var info = _repository.Get(x => x.Id == id).Single();

            _navigationService.Navigate(Experiences.Main.ToString(), null);
            _eventAggregator.GetEvent<AddCityMessage>().Publish(info);
        }

        private static SearchResult ConvertCityToSearchResult(CityInfo r)
        {
            var sign = r.CurrentOffset < TimeSpan.Zero ? string.Empty : "+";
            return new SearchResult
            {
                Id = r.Id,
                Title = r.Name,
                Subtitle = r.State,
                Description = r.CountryName + " " + string.Format("{0}{1:00}:{2:00} UTC", sign,  r.CurrentOffset.Hours, r.CurrentOffset.Minutes),
                Image = new Uri(string.Format("ms-appx:///Assets/CountryFlags/{0}.png", r.CountryCode))
            };
        }
    }
}