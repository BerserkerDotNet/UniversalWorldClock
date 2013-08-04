using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Views;
using Windows.ApplicationModel.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalWorldClock.ViewModels
{
    public sealed class MainViewModel :  ViewModelBase
    {
        #region Fields
        private ViewModelBase _currentContent;
        private ObservableCollection<ClockInfo> _clocks;
        private IDataRepository<ClockInfo> _clocksRepository;
        private IDataRepository<CityInfo> _citiesRepository;
        private SearchPane _searchPane = SearchPane.GetForCurrentView();
        private IEnumerable<CityInfo> _cities;
        private TimeSpan _globalTimeShift; 
        #endregion

        #region Public Methods
        public MainViewModel(IDataRepository<CityInfo> citiesRepository, IDataRepository<ClockInfo> clocksRepository)
        {
            _citiesRepository = citiesRepository;
            _clocksRepository = clocksRepository;
            Initialize();
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            CurrentContent = viewModel;
        }

        public void AddClock(ClockInfo info)
        {
            if (!Clocks.Contains(info))
                Clocks.Add(info);

            _clocksRepository.Save(Clocks);
        }

        public void DeleteClock(ClockInfo clock)
        {
            if (!Clocks.Contains(clock))
                return;

            Clocks.Remove(clock);
            _clocksRepository.Save(Clocks);
        } 
        #endregion

        #region Public Properties
        public ICommand ClearTimeShift { get; private set; }
        public ICommand Add { get; private set; }

        public ViewModelBase CurrentContent
        {
            get { return _currentContent; }
            set
            {
                if (_currentContent != value)
                {
                    _currentContent = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ClockInfo> Clocks
        {
            get { return _clocks; }
            set
            {
                if (_clocks != value)
                {
                    _clocks = value;
                    OnPropertyChanged();
                }
            }
        }

        public int GlobalHourTimeShift
        {
            get { return _globalTimeShift.Hours; }
            set
            {
                var isHoursBecomesNegative = (value < 0 && _globalTimeShift.Minutes > 0);
                var isHoursBecomesPositiveOrZero = (value >= 0 && _globalTimeShift.Minutes < 0);
                var minutes = isHoursBecomesNegative || isHoursBecomesPositiveOrZero
                                  ? -_globalTimeShift.Minutes
                                  : _globalTimeShift.Minutes;

                _globalTimeShift = new TimeSpan(0, value, minutes, 0);
                OnPropertyChanged();
                OnPropertyChanged("TimeShift");
            }
        }

        public int GlobalMinuteTimeShift
        {
            get { return _globalTimeShift.Minutes; }
            set
            {
                var minutes = _globalTimeShift.Hours < 0 ? -value : value;
                _globalTimeShift = new TimeSpan(0, _globalTimeShift.Hours, minutes,
                                                0);
                OnPropertyChanged();
                OnPropertyChanged("TimeShift");
            }
        }

        public TimeSpan TimeShift { get { return _globalTimeShift; } }

        #endregion

        #region Private Methods
        private async void Initialize()
        {
            SearchPaneSetup();
            ClearTimeShift = new RelayCommand(() =>
                                                  {
                                                      GlobalHourTimeShift = 0;
                                                      GlobalMinuteTimeShift = 0;
                                                  });
            Add = new RelayCommand(() => SearchPane.GetForCurrentView().Show());
            var clocks = await _clocksRepository.Get();
            _cities = await _citiesRepository.Get();

            //NOTE: temp fix for time zone Id
            foreach (var clockInfo in clocks)
            {
                var cities = _cities.Where(c => c.Name.Equals(clockInfo.CityName) && c.CountryName.Equals(clockInfo.CountryName) && c.CountryCode.Equals(clockInfo.CountryCode));
                if (cities.Count() > 1 || !cities.Any())
                    continue;
                clockInfo.TimeZoneId = cities.Single().TimeZoneId;
            }
            _clocksRepository.Save(clocks);
            Clocks = new ObservableCollection<ClockInfo>(clocks);
            SearchPaneSetup();


        }

        private void SearchPaneSetup()
        {
            _searchPane.PlaceholderText = "City name";
            _searchPane.ShowOnKeyboardInput = true;
            _searchPane.QuerySubmitted += OnSearchPaneQuerySubmitted;
            _searchPane.SuggestionsRequested += OnSearchPaneSuggestionsRequested;
            _searchPane.ResultSuggestionChosen += OnSearchPaneResultSuggestionChosen;
        }

        private void OnSearchPaneResultSuggestionChosen(SearchPane sender, SearchPaneResultSuggestionChosenEventArgs args)
        {
            var id = int.Parse(args.Tag);
            var result = _cities.Single(x => x.Id == id);
            var info = new ClockInfo
                           {
                               CityName = result.Name,
                               TimeZoneId = result.TimeZoneId,
                               CountryCode = result.CountryCode,
                               CountryName = result.CountryName
                           };
            AddClock(info);
        }

        private void OnSearchPaneSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            var request = args.Request;
            var matches = _cities.Where(x => x.Name.StartsWith(args.QueryText, StringComparison.OrdinalIgnoreCase)).Take(10);
            request.SearchSuggestionCollection.AppendQuerySuggestions(matches.Select(m => m.Name));

            var resultSuggestion = matches.Where(x => x.Name.Equals(args.QueryText, StringComparison.OrdinalIgnoreCase)).Take(5);
            if (!resultSuggestion.Any())
                return;

            var image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/city.png"));
            foreach (var match in resultSuggestion)
            {
                request.SearchSuggestionCollection.AppendResultSuggestion(match.Name,
                                                                          match.CountryName + ", " + match.State,
                                                                          match.Id.ToString(), image, match.CountryName);
            }
        }

        private void OnSearchPaneQuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            frame.Navigate(typeof(CitiesSearchResultsPage), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        } 
        #endregion
    }
}
