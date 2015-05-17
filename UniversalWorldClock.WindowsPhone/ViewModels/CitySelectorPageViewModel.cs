using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Messages;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.ViewModels
{
    public class CitySelectorPageViewModel : ViewModel, ICitySelectorViewModel
    {
        private readonly IDataRepository _repository;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigation;
        private ObservableCollection<CityInfo> _cities;
        private string _query;

        public CitySelectorPageViewModel(IDataRepository repository, IEventAggregator eventAggregator, INavigationService navigation)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;
            _navigation = navigation;
            Cities = new ObservableCollection<CityInfo>(_repository.GetAll());

            ApplyFilter = new DelegateCommand<AutoSuggestBoxTextChangedEventArgs>( ApplyFilterInternal);
            AddCity = new DelegateCommand<AutoSuggestBoxSuggestionChosenEventArgs>(AddCityInternal);

        }

        public ObservableCollection<CityInfo> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }

        public ICommand ApplyFilter { get; set; }

        public ICommand AddCity { get; set; }

        public string Query
        {
            get { return _query; }
            set { SetProperty(ref _query, value); }
        }

        private void FilterCountries(string query)
        {
            var filtered = _repository.GetAll()
                .Where(c => c.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Cities = new ObservableCollection<CityInfo>(filtered);
        }

        private void AddCityInternal(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var info = args.SelectedItem as CityInfo;
            if (info != null)
            {
                _navigation.Navigate(Experiences.Main.ToString(), null);
                _eventAggregator.GetEvent<AddCityMessage>().Publish(info);
            }
            
        }

        private void ApplyFilterInternal(AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                FilterCountries(Query);
            }
        }
    }
}
