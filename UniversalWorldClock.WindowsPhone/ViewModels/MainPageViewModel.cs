using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Messages;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.Tasks;
using UniversalWorldClock.ViewModels.Interfaces;
using UniversalWorldClock.Views;


namespace UniversalWorldClock.ViewModels
{
    public sealed class MainPageViewModel : ViewModel, IMainPageViewModel
    {
        private ObservableCollection<CityInfo> _clocks;
        private readonly IDataRepository _citiesRepository;
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _eventAggregator;
        private bool _menuActive;

        public MainPageViewModel(IDataRepository citiesRepository, INavigationService navigation, IEventAggregator eventAggregator)
        {
            _citiesRepository = citiesRepository;
            _navigation = navigation;
            _eventAggregator = eventAggregator;
            var clocks = _citiesRepository.GetUsersCities();
            Clocks = new ObservableCollection<CityInfo>(clocks);
            _eventAggregator.GetEvent<AddCityMessage>().Subscribe(HandleAddCity, true);
            _eventAggregator.GetEvent<DeleteCityMessage>().Subscribe(HandleDeleteCity, true);
            
            Add = new RelayCommand(() =>
            {
                _navigation.Navigate(Experiences.CitySelector.ToString(), null);
            });
            Donate = new RelayCommand(() => Launcher.LaunchUriAsync(new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=UFS2JX3EJGU3N")));
            GoToDetails = new DelegateCommand<CityInfo>(HandleGotoDetails);
            SetShowMenu = new RelayCommand(() =>
            {
                MenuActive = !MenuActive;
            });
            GoToSettings = new RelayCommand(() =>
            {
                _navigation.Navigate(Experiences.Settings.ToString(), null);
            });
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            Clocks.CollectionChanged += Clocks_CollectionChanged;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            if (MenuActive)
                MenuActive = false;
            _eventAggregator.GetEvent<BeforeHideClocksMessage>().Publish(null);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            Clocks.CollectionChanged -= Clocks_CollectionChanged;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (_menuActive)
            {
                e.Handled = true;
                MenuActive = !_menuActive;
            }

        }

        private void HandleGotoDetails(CityInfo cityInfo)
        {
            if (MenuActive)
            {
                MenuActive = false;
                return;
            }

            if (cityInfo != null)
            {
                _navigation.Navigate(Experiences.CityDetails.ToString(), cityInfo);
                Clocks.CollectionChanged -= Clocks_CollectionChanged;
            }
        }

        public ICommand Add { get; private set; }
        public ICommand SetTime { get; private set; }
        public ObservableCollection<CityInfo> Clocks
        {
            get { return _clocks; }
            set { SetProperty(ref _clocks, value); }
        }
        public ICommand Donate { get; private set; }
        public bool MenuActive
        {
            get { return _menuActive; }
            set
            {
                SetProperty(ref _menuActive, value);
                (Application.Current as App).SuppressBackButton = _menuActive;
            }
        }

        public ICommand SetShowMenu { get; private set; }
        public ICommand GoToSettings { get; private set; }
        public ICommand GoToDetails { get; private set; }

        private void HandleDeleteCity(CityInfo info)
        {
            if (!Clocks.Contains(info))
                return;

            Clocks.Remove(info);

            //NOTE: Need a way to cleanup 
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

        private void HandleAddCity(CityInfo info)
        {
            if (!Clocks.Contains(info))
                Clocks.Add(info);
        }
    }
}
