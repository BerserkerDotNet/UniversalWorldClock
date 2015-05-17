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
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using UniversalWorldClock.Messages;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.ViewModels
{
    public sealed class MainPageViewModel :  ViewModel, IMainPageViewModel
    {
        private ObservableCollection<CityInfo> _clocks;
        private readonly IDataRepository _citiesRepository;
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _eventAggregator;
        private bool _showMenu;

        public MainPageViewModel(IDataRepository citiesRepository, INavigationService navigation, IEventAggregator eventAggregator)
        {
            _citiesRepository = citiesRepository;
            _navigation = navigation;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AddCityMessage>().Subscribe(HandleAddCity, true);
            _eventAggregator.GetEvent<DeleteCityMessage>().Subscribe(HandleDeleteCity, true);
            var clocks = _citiesRepository.GetUsersCities();
            Clocks = new ObservableCollection<CityInfo>(clocks);
            
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            Donate = new RelayCommand(() => Launcher.LaunchUriAsync(new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=UFS2JX3EJGU3N")));
            SetShowMenu = new RelayCommand(() => MenuActive = !MenuActive);

            Clocks.CollectionChanged += Clocks_CollectionChanged;
        }
        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            Clocks.CollectionChanged -= Clocks_CollectionChanged;
            _eventAggregator.GetEvent<BeforeHideClocksMessage>().Publish(null);
        }

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
                    OnPropertyChanged(()=>Clocks);
                }
            }
        }
        public ICommand Donate { get; private set; }

        public bool MenuActive
        {
            get { return _showMenu; }
            set { SetProperty(ref _showMenu, value); }
        }

        public ICommand SetShowMenu { get; private set; }
        public ICommand GoToSettings { get; private set; }
        public ICommand GoToDetails { get; private set; }

        private void HandleAddCity(CityInfo info)
        {
            if (!Clocks.Contains(info))
                Clocks.Add(info);
        }
        private void HandleDeleteCity(CityInfo clock)
        {
            if (!Clocks.Contains(clock))
                return;

            Clocks.Remove(clock);

            //NOTE: Need a way to cleanup 
        }
        private void Clocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnClocksCollectionChanged();
        }

        private void OnClocksCollectionChanged()
        {
            var updater = new LiveTileScheduler();
            _citiesRepository.Save(Clocks);
            Task.Factory.StartNew(() => RescheduleTileUpdate(updater), TaskCreationOptions.LongRunning);
        }

        private void RescheduleTileUpdate(LiveTileScheduler updater)
        {
            updater.ReSchedule(Clocks.Select(c => new Tasks.Domain.CityInfo
            {
                Id = c.Id,
                Name = c.Name,
                TimeZoneId = c.TimeZoneId
            }));
        }
        public void OnSearchPaneResultSuggestionChosen(SearchBox sender, SearchBoxResultSuggestionChosenEventArgs args)
        {
            var id = int.Parse(args.Tag);
            var result = _citiesRepository.Get(x => x.Id == id).Single();
            HandleAddCity(result);
        }

        public void OnSearchPaneSuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            var request = args.Request;
            var matches = _citiesRepository.Get(x => x.Name.StartsWith(args.QueryText, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();
            var matchesTimeZones = _citiesRepository.GetAll().GroupBy(x => x.TimeZoneId).Where(x => x.Key.StartsWith(args.QueryText, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();
            request.SearchSuggestionCollection.AppendQuerySuggestions(matches.Select(m => m.Name));
            request.SearchSuggestionCollection.AppendQuerySuggestions(matchesTimeZones.Select(m => m.Key));

            var resultSuggestion = matches.Where(x => x.Name.Equals(args.QueryText, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();
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
        public void OnSearchPaneQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            _navigation.Navigate(Experiences.CitiesSearchResults.ToString(), args.QueryText);
        } 
    }
}
