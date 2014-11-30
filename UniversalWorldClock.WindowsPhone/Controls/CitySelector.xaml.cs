using System;
using Windows.UI.Xaml.Controls;
using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels;

namespace UniversalWorldClock.Controls
{
    public sealed partial class CitySelector : UserControl
    {
        public CitySelector()
        {
            this.InitializeComponent();
        }

        public void ClearText()
        {
            uiClockSelectorBox.Text = string.Empty;
        }

        private void AutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var mainViewModel = DataContext as MainViewModel;
                if (mainViewModel != null) mainViewModel.FilterCountries(sender.Text);
            }
        }

        private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = args.SelectedItem as CityInfo;
            var mainViewModel = DataContext as MainViewModel;
            if (mainViewModel != null) mainViewModel.AddClock(item);
            OnCitySelected();
        }

        private void OnCitySelected()
        {
            var handler = CitySelected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler CitySelected;
    }
}
