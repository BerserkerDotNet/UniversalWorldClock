using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Microsoft.Practices.Prism.Mvvm;
using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels.Interfaces;

namespace UniversalWorldClock.Views
{
    public sealed partial class MainPage: IView
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ListView_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
                listView.ReorderMode = ListViewReorderMode.Enabled;
        }

        private void PopupMenu_OnLoaded(object sender, RoutedEventArgs e)
        {
            var popup = (sender as Popup);
            if(popup==null)
                return;
            popup.VerticalOffset = Window.Current.Bounds.Height - 380;
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ExecuteGoToDetails((CityInfo)e.ClickedItem);
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var listView = (ListView)sender;
            if (listView.ReorderMode == ListViewReorderMode.Enabled)
                return;
            ExecuteGoToDetails((CityInfo)listView.SelectedItem);
        }

        private void ExecuteGoToDetails(CityInfo item)
        {
            ((IMainPageViewModel)DataContext).GoToDetails.Execute(item);
        }
    }
}
