using System.Collections.ObjectModel;
using System.Dynamic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Media.Imaging;
using FoulPlay_Windows8.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using FoulPlay_Windows8.Tools;
using Newtonsoft.Json;

namespace FoulPlay_Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private static UserAccountEntity.User _user;
        public static FriendScrollingCollection FriendCollection { get; set; }
        private static RecentActivityManager _recentActivityManager = new RecentActivityManager();
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            CreateMenu();
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null && e.PageState.ContainsKey("userEntity"))
            {
                var jsonObjectString = e.PageState["userAccountEntity"].ToString();
                App.UserAccountEntity = JsonConvert.DeserializeObject<UserAccountEntity>(jsonObjectString);
                jsonObjectString = e.PageState["userEntity"].ToString();
                var user = JsonConvert.DeserializeObject<UserAccountEntity.User>(jsonObjectString);
                App.UserAccountEntity.SetUserEntity(user);
            }
            _user = App.UserAccountEntity.GetUserEntity();
            GetFriendsList(true, false, false, false, true, false, false);
            LoadRecentActivityList();
            LoadMessages();
        }

        private void CreateMenu()
        {
            var resourceLoader = ResourceLoader.GetForCurrentView(); 
            List<MenuItem> menuItems = new List<MenuItem>();
            menuItems.Add(new MenuItem("/Assets/phone_trophy_icon_compareTrophies.png", resourceLoader.GetString("RecentActivity/Text"), string.Empty));
            menuItems.Add(new MenuItem("/Assets/phone_trophy_icon_compareTrophies.png", resourceLoader.GetString("TrophyHeader/Text"), string.Empty));
            menuItems.Add(new MenuItem("/Assets/phone_trophy_icon_compareTrophies.png", resourceLoader.GetString("LiveFromPlaystation/Text"), string.Empty));
            menuItems.Add(new MenuItem("/Assets/phone_trophy_icon_compareTrophies.png", resourceLoader.GetString("ProfileHeader/Text"), "profile"));
            MenuGridView.ItemsSource = menuItems;
        }

        private class MenuItem
        {
            public string Text { get; private set; }

            public string Location { get; private set; }

            public string Icon { get; private set; }

            

            public MenuItem(string icon, string text, string location)
            {
                Text = text;
                Icon = icon;
                Location = location;
            }
        }

        private async void LoadRecentActivityList()
        {
            //LoadingProgressBar.Visibility = Visibility.Visible;
            var recentActivityCollection = new RecentActivityScrollingCollection
            {
                IsNews = true,
                StorePromo = true,
                UserAccountEntity = App.UserAccountEntity,
                Username = _user.OnlineId,
                PageCount = 1
            };
            var recentActivityManager = new RecentActivityManager();
            var recentActivityEntity =
                await recentActivityManager.GetActivityFeed(_user.OnlineId, 0, true, true, App.UserAccountEntity);
            if (recentActivityEntity == null)
            {
                //recentActivityCollection = null;
                //ActivityFeedListView.DataContext = recentActivityCollection;
                //NoActivitiesTextBlock.Visibility = Visibility.Visible;
                //LoadingProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            if (recentActivityEntity.feed != null)
            {
                //NoActivitiesTextBlock.Visibility = Visibility.Collapsed;
                foreach (var item in recentActivityEntity.feed)
                {
                    recentActivityCollection.Add(item);
                }
            }
            DefaultViewModel["RecentActivityList"] = recentActivityCollection;
            //RecentActivityHubSection.DataContext = recentActivityCollection;
            //LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            string jsonObjectString = JsonConvert.SerializeObject(App.UserAccountEntity);
            e.PageState["userAccountEntity"] = jsonObjectString;
            jsonObjectString = JsonConvert.SerializeObject(App.UserAccountEntity.GetUserEntity());
            e.PageState["userEntity"] = jsonObjectString;
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
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

        public async void GetFriendsList(bool onlineFilter, bool blockedPlayer, bool recentlyPlayed,
            bool personalDetailSharing, bool friendStatus, bool requesting, bool requested)
        {
            var friendManager = new FriendManager();
            FriendCollection = new FriendScrollingCollection
            {
                UserAccountEntity = App.UserAccountEntity,
                Offset = 32,
                OnlineFilter = onlineFilter,
                Requested = requested,
                Requesting = requesting,
                PersonalDetailSharing = personalDetailSharing,
                FriendStatus = friendStatus,
                Username = _user.OnlineId
            };
            var items =
                await
                    friendManager.GetFriendsList(_user.OnlineId, 0, blockedPlayer, recentlyPlayed, personalDetailSharing,
                        friendStatus, requesting, requested, onlineFilter, App.UserAccountEntity);
            if (items == null)
            {
                //FriendsMessageTextBlock.Visibility = Visibility.Visible;
                //FriendsLongListSelector.DataContext = FriendCollection;
                return;
            }
            if (items.FriendList == null)
            {
                FriendsProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            //FriendsMessageTextBlock.Visibility = Visibility.Collapsed;
            //FriendsMessageTextBlock.Visibility = !items.FriendList.Any() ? Visibility.Visible : Visibility.Collapsed;
            foreach (var item in items.FriendList)
            {
                FriendCollection.Add(item);
            }
            //FriendsLongListSelector.ItemRealized += friendList_ItemRealized;
            DefaultViewModel["FriendList"] = FriendCollection;
            FriendsProgressBar.Visibility = Visibility.Collapsed;
            return;
        }

        private async void LoadMessages()
        {
            MessageProgressBar.Visibility = Visibility.Visible;
            var messageManager = new MessageManager();
            MessageGroupEntity message = await messageManager.GetMessageGroup(_user.OnlineId, App.UserAccountEntity);
            //MessagesMessageTextBlock.Visibility = message != null && (message.MessageGroups != null && (!message.MessageGroups.Any()))
            //    ? Visibility.Visible
            //    : Visibility.Collapsed;
            MessagesListView.DataContext = message;
            MessageProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox == null) return;
            switch (FilterComboBox.SelectedIndex)
            {
                case 0:
                    // Friends - Online
                    GetFriendsList(true, false, false, false, true, false, false);
                    break;
                case 1:
                    // All
                    GetFriendsList(false, false, false, false, true, false, false);
                    break;
                case 2:
                    // Friend Request Received
                    GetFriendsList(false, false, false, false, true, false, true);
                    break;
                case 3:
                    // Friend Requests Sent
                    GetFriendsList(false, false, false, false, true, true, false);
                    break;
                case 4:
                    // Name Requests Received
                   GetFriendsList(true, false, false, true, true, false, false);
                    break;
                case 5:
                    // Name Requests Sent
                    GetFriendsList(false, false, false, true, true, true, false);
                    break;
            }
        }
        private async void MenuGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MenuItem;
            if (item == null) return;
            switch (item.Location)
            {
                case "profile":
                    var user = await UserManager.GetUser(_user.OnlineId, App.UserAccountEntity);
                    string jsonObjectString = JsonConvert.SerializeObject(user);
                    Frame.Navigate(typeof(FriendPage), jsonObjectString);
                    break;
            }
        }

        private async void FriendsListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FriendsEntity.Friend;
            if (item == null) return;
            var user = await UserManager.GetUser(item.OnlineId, App.UserAccountEntity);
            string jsonObjectString = JsonConvert.SerializeObject(user);
            Frame.Navigate(typeof(FriendPage), jsonObjectString);
        }
    }
}
