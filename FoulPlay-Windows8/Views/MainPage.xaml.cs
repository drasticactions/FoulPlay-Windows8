// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FoulPlay.Core.Entities;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using Foulplay_Windows8.Core.Tools;
using FoulPlay_Windows8.Tools;
using FoulPlay_Windows8.UserControls;
using FoulPlay_Windows8.ViewModels;
using Newtonsoft.Json;

namespace FoulPlay_Windows8.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page, IDisposable
    {
        private static UserAccountEntity.User _user;
        private static RecentActivityManager _recentActivityManager = new RecentActivityManager();
        private readonly NavigationHelper navigationHelper;
        private MainPageViewModel _vm;


        public MainPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            CreateMenu();
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;
        }

        public static FriendScrollingCollection FriendCollection { get; set; }

        /// <summary>
        ///     NavigationHelper is used on each page to aid in navigation and
        ///     process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public void Dispose()
        {
        }

        /// <summary>
        ///     Populates the page with content passed during navigation. Any saved state is also
        ///     provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        ///     The source of the event; typically <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     Event data that provides both the navigation parameter passed to
        ///     <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested and
        ///     a dictionary of state preserved by this page during an earlier
        ///     session. The state will be null the first time a page is visited.
        /// </param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _vm = (MainPageViewModel)DataContext;
            if (e.PageState != null && e.PageState.ContainsKey("userEntity"))
            {
                string jsonObjectString = e.PageState["userAccountEntity"].ToString();
                App.UserAccountEntity = JsonConvert.DeserializeObject<UserAccountEntity>(jsonObjectString);
                jsonObjectString = e.PageState["userEntity"].ToString();
                var user = JsonConvert.DeserializeObject<UserAccountEntity.User>(jsonObjectString);
                App.UserAccountEntity.SetUserEntity(user);
            }
            _user = App.UserAccountEntity.GetUserEntity();
            _vm.SetFriendsList(_user.OnlineId, true, false, false, false, true, false, false);
            _vm.SetRecentActivityFeed(_user.OnlineId);
            _vm.SetMessages(_user.OnlineId, App.UserAccountEntity);
            _vm.SetInviteList();
            CreateBackgroundTask();
        }

        private async void CreateBackgroundTask()
        {
            // We have login, so set up the background tasks.
            BackgroundTaskUtils.UnregisterBackgroundTasks(BackgroundTaskUtils.BackgroundTaskName);
            BackgroundTaskRegistration task = await
                BackgroundTaskUtils.RegisterBackgroundTask(BackgroundTaskUtils.BackgroundTaskEntryPoint,
                    BackgroundTaskUtils.BackgroundTaskName,
                    new TimeTrigger(15, false),
                    null);
        }

        private void CreateMenu()
        {
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            var menuItems = new List<MainPageViewModel.MenuItem>();
            menuItems.Add(new MainPageViewModel.MenuItem("/Assets/phone_home_footerIcon_region.png",
                resourceLoader.GetString("RecentActivity/Text"), "recent"));
            menuItems.Add(new MainPageViewModel.MenuItem("/Assets/appbar.film.png", resourceLoader.GetString("LiveFromPlaystation/Text"),
                "live"));
            menuItems.Add(new MainPageViewModel.MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
                resourceLoader.GetString("ProfileHeader/Text"), "profile"));
            MenuGridView.ItemsSource = menuItems;
        }

        /// <summary>
        ///     Preserves state associated with this page in case the application is suspended or the
        ///     page is discarded from the navigation cache.  Values must conform to the serialization
        ///     requirements of <see cref="SuspensionManager.SessionState" />.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper" /></param>
        /// <param name="e">
        ///     Event data that provides an empty dictionary to be populated with
        ///     serializable state.
        /// </param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            string jsonObjectString = JsonConvert.SerializeObject(App.UserAccountEntity);
            e.PageState["userAccountEntity"] = jsonObjectString;
            jsonObjectString = JsonConvert.SerializeObject(App.UserAccountEntity.GetUserEntity());
            e.PageState["userEntity"] = jsonObjectString;
        }

        private async void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFriendList();
        }

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

        private async void MenuGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MainPageViewModel.MenuItem;
            if (item == null) return;
            switch (item.Location)
            {
                case "profile":
                    Frame.Navigate(typeof (FriendPage), _user.OnlineId);
                    break;
                case "live":
                    Frame.Navigate(typeof (LiveFromPlaystationPage));
                    break;
                case "recent":
                    Frame.Navigate(typeof (RecentActivityPage));
                    break;
            }
        }

        private void FriendsListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FriendsEntity.Friend;
            if (item == null) return;
            Frame.Navigate(typeof (FriendPage), item.OnlineId);
        }

        private void MessagesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MainPageViewModel.MessageGroupItem;
            if (item == null) return;
            string jsonObjectString = JsonConvert.SerializeObject(item.MessageGroup);
            Frame.Navigate(typeof (MessagePage), jsonObjectString);
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

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="GridCS.Common.NavigationHelper.LoadState" />
        /// and
        /// <see cref="GridCS.Common.NavigationHelper.SaveState" />
        /// .
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void MessagesRefreshAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.SetMessages(_user.OnlineId, App.UserAccountEntity);
        }

        private void FriendsRefreshAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            SetFriendList();
        }

        private void InvitesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SessionInviteEntity.Invitation;
            if (item == null) return;
            var control = new SessionInviteUserControl();
            control.SetOffset();
            control.SetContext(item);
            control.OpenPopup();
        }

        private void GameInviteRefreshAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.SetInviteList();
        }
    }
}