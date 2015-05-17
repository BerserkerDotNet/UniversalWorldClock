using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using UniversalWorldClock.Models;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.ViewModels.DesignTime
{
    public class DesignCitiesSearchResultsPageViewModel : ICitiesSearchResultsPageViewModel
    {
        public ObservableCollection<SearchFilter> Filters
        {
            get { return new ObservableCollection<SearchFilter>(new []
            {
                new SearchFilter("Filter1", 10, true), 
                new SearchFilter("Filter2", 5, false), 
                new SearchFilter("Filter3", 2, false), 
            });}
            set {  }
        }

        public ObservableCollection<SearchResult> Results
        {
            get { return new ObservableCollection<SearchResult>(new[]
            {
                new SearchResult
                {
                    Id = 1,
                    Description = "Test city",
                    Title = "City",
                    Subtitle = "The city",
                    Image = new Uri("ms-appx:///Assets/CountryFlags/UA.png")
                } 
            } );
        }
            set {  }
        }

        public bool ShowFilters
        {
            get { return true; }
            set {  }
        }

        public bool IsInProgress
        {
            get { return true; }
            set {  }
        }

        public string QueryText
        {
            get { return "Test query"; }
            set {  }
        }

        public SearchFilter CurrentFilter
        {
            get { return Filters.First(); }
            set {  }
        }

        public ICommand ApplyFilter
        {
            get { return new RelayCommand(() => { }); }
            set {  }
        }

        public ICommand SelectFilter
        {
            get { return new RelayCommand(()=>{});}
            set {  }
        }

        public ICommand AddCity
        {
            get { return new RelayCommand(() => { });}
            set { }
        }
    }
}