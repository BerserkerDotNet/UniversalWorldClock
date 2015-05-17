using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Practices.Prism.StoreApps;
using UniversalWorldClock.Common;
using UniversalWorldClock.Data;
using UniversalWorldClock.Models;

namespace UniversalWorldClock.Views
{
    public sealed partial class CitiesSearchResultsPage : VisualStateAwarePage 
    {
        public CitiesSearchResultsPage()
        {
            this.InitializeComponent();
        }

        protected override string DetermineVisualState(double width, double height)
        {
            return ViewStateHelper.GetViewState(width).ToString();
        }

        private void ResultsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
