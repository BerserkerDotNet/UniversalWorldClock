using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.Tasks;
using UniversalWorldClock.Views;
using Windows.ApplicationModel.Search;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace UniversalWorldClock.ViewModels
{
    public sealed class MainViewModel :  ViewModelBase
    {
        #region Fields
        private ObservableCollection<CityInfo> _clocks;
        private IDataRepository<CityInfo> _citiesRepository;
        private SearchPane _searchPane;
        #endregion

        #region Public Methods
        public MainViewModel(IDataRepository<CityInfo> citiesRepository)
        {
            _citiesRepository = citiesRepository;
            Initialize();
        }

        public void AddClock(CityInfo info)
        {
            if (!Clocks.Contains(info))
                Clocks.Add(info);
        }
        public void DeleteClock(CityInfo clock)
        {
            if (!Clocks.Contains(clock))
                return;

            Clocks.Remove(clock);

            //NOTE: Need a way to cleanup 
        } 

        #endregion

        #region Public Properties

        public ICommand Add { get; private set; }
        public ICommand SetTime { get; private set; }

        public ObservableCollection<CityInfo> Clocks
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

        public ICommand Donate { get; private set; }

        #endregion

        #region Private Methods
        private async void Initialize()
        {
            Add = new RelayCommand(() => SearchPane.GetForCurrentView().Show());
            Donate = new RelayCommand(() => Launcher.LaunchUriAsync(new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=UFS2JX3EJGU3N")));

            var clocks = _citiesRepository.GetUsersCities();
            Clocks = new ObservableCollection<CityInfo>(clocks);
            Clocks.CollectionChanged += Clocks_CollectionChanged;
            SearchPaneSetup();
        }

        private void Clocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _citiesRepository.Save(Clocks);
            var updater = new LiveTileScheduler();

            Task.Factory.StartNew(() =>
                                  {
                                      updater.ReSchedule(Clocks.Select(c => new Tasks.Domain.CityInfo
                                                                            {
                                                                                Id = c.Id,
                                                                                Name = c.Name,
                                                                                TimeZoneId = c.TimeZoneId
                                                                            }));
                                  }, TaskCreationOptions.LongRunning);

        }

        private void SearchPaneSetup()
        {
            _searchPane = SearchPane.GetForCurrentView();
            _searchPane.PlaceholderText = "City name or time zone";
            _searchPane.ShowOnKeyboardInput = true;
            _searchPane.QuerySubmitted += OnSearchPaneQuerySubmitted;
            _searchPane.SuggestionsRequested += OnSearchPaneSuggestionsRequested;
            _searchPane.ResultSuggestionChosen += OnSearchPaneResultSuggestionChosen;
        }

        private async void OnSearchPaneResultSuggestionChosen(SearchPane sender, SearchPaneResultSuggestionChosenEventArgs args)
        {
            var id = int.Parse(args.Tag);
            var result = _citiesRepository.Get(x => x.Id == id).Single();
            AddClock(result);
        }

        private void OnSearchPaneSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            var request = args.Request;
            var matches = _citiesRepository.Get(x => x.Name.StartsWith(args.QueryText, StringComparison.OrdinalIgnoreCase)).Take(10);
            var matchesTimeZones = _citiesRepository.GetAll().GroupBy(x => x.TimeZoneId).Where(x => x.Key.StartsWith(args.QueryText, StringComparison.OrdinalIgnoreCase)).Take(10);
            request.SearchSuggestionCollection.AppendQuerySuggestions(matches.Select(m => m.Name));
            request.SearchSuggestionCollection.AppendQuerySuggestions(matchesTimeZones.Select(m => m.Key));

            var resultSuggestion = matches.Where(x => x.Name.Equals(args.QueryText, StringComparison.OrdinalIgnoreCase)).Take(5);
            if (!resultSuggestion.Any())
                return;

            
            foreach (var match in resultSuggestion)
            {
                var image = RandomAccessStreamReference.CreateFromUri(new Uri(string.Format("ms-appx:///Assets/CountryflagsTile/{0}.png", match.CountryCode)));
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

        public void Refresh()
        {
            ManualPropertyChanged("Clocks");
        }
    }
}
