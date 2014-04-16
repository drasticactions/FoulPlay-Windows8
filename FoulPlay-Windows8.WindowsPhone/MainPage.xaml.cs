using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using FoulPlay_Windows8.UserControls;
using FoulPlay_Windows8.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641
using FoulPlay_Windows8.Views;
using Newtonsoft.Json;

namespace FoulPlay_Windows8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _vm;
        private static UserAccountEntity.User _user;
        private readonly NavigationHelper _navigationHelper;
        public MainPage()
        {
            this.InitializeComponent();
            
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            this._navigationHelper = new NavigationHelper(this);
            this._navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this._navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null && e.PageState.ContainsKey("userEntity"))
            {
                string jsonObjectString = e.PageState["userAccountEntity"].ToString();
                App.UserAccountEntity = JsonConvert.DeserializeObject<UserAccountEntity>(jsonObjectString);
                jsonObjectString = e.PageState["userEntity"].ToString();
                var user = JsonConvert.DeserializeObject<UserAccountEntity.User>(jsonObjectString);
                App.UserAccountEntity.SetUserEntity(user);
            }
            _vm.CreateMenuPhone();
            _user = App.UserAccountEntity.GetUserEntity();
            _vm.SetRecentActivityFeed(_user.OnlineId);
            _vm.SetMessages(_user.OnlineId, App.UserAccountEntity);
            _vm.SetFriendsList(_user.OnlineId, true, false, false, false, true, false, false);
            _vm.SetInviteList();
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        private void MenuGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MainPageViewModel.MenuItem;
            if (item == null) return;
            switch (item.Location)
            {
                case "profile":
                    //Frame.Navigate(typeof(FriendPage), _user.OnlineId);
                    break;
                case "live":
                    Frame.Navigate(typeof(LiveFromPlayStationPage));
                    break;
                case "recent":
                    //Frame.Navigate(typeof(RecentActivityPage));
                    break;
            }
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _vm = (MainPageViewModel)DataContext;
            this._navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void SetFriendList()
        {
            if (FilterComboBox == null) return;
            switch (FilterComboBox.SelectedIndex)
            {
                case 0:
                    // Friends - Online
                    _vm.SetFriendsList(_user.OnlineId, true, false, false, false, true, false, false);
                    break;
                case 1:
                    // All
                    _vm.SetFriendsList(_user.OnlineId, false, false, false, false, true, false, false);
                    break;
                case 2:
                    // Friend Request Received
                    _vm.SetFriendsList(_user.OnlineId, false, false, false, false, true, false, true);
                    break;
                case 3:
                    // Friend Requests Sent
                    _vm.SetFriendsList(_user.OnlineId, false, false, false, false, true, true, false);
                    break;
                case 4:
                    // Name Requests Received
                    _vm.SetFriendsList(_user.OnlineId, true, false, false, true, true, false, false);
                    break;
                case 5:
                    // Name Requests Sent
                    _vm.SetFriendsList(_user.OnlineId, false, false, false, true, true, true, false);
                    break;
            }
        }
        
        private void ActivityFeedGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RecentActivityEntity.Feed;
            if (item == null) return;
            var control = new RecentActivityUserControl();
            control.SetOffset();
            control.SetContext(item);
            control.OpenPopup();
        }

        private void MessagesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MessagesRefreshAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FriendsListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FriendsEntity.Friend;
            if (item == null) return;
            Frame.Navigate(typeof(FriendPage), item.OnlineId);
        }

        private void InvitesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFriendList();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (MainPivot.SelectedIndex)
            {
                case 1:
                    SetFriendList();
                    break;
                case 2:
                    _vm.SetMessages(_user.OnlineId, App.UserAccountEntity);
                    break;
                case 3:
                    _vm.SetInviteList();
                    break;
            }
        }
    }
}
