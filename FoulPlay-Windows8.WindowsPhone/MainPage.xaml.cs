// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641
using System;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FoulPlay.Core.Entities;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using Foulplay_Windows8.Core.Tools;
using FoulPlay_Windows8.UserControls;
using FoulPlay_Windows8.ViewModels;
using FoulPlay_Windows8.Views;
using Newtonsoft.Json;

namespace FoulPlay_Windows8
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private UserAccountEntity.User _user;
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private LiveFromPlaystationPageViewModel _liveVm;
        private MainPageViewModel _vm;

        public MainPage()
        {
            InitializeComponent();

            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            _vm = (MainPageViewModel)DataContext;
            _liveVm = (LiveFromPlaystationPageViewModel)LiveFromPlaystationGrid.DataContext;
            NavigationHelper = new NavigationHelper(this);
            NavigationHelper.LoadState += navigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;
        }

        /// <summary>
        ///     Gets the <see cref="NavigationHelper" /> associated with this <see cref="Page" />.
        /// </summary>
        public NavigationHelper NavigationHelper { get; private set; }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null && e.PageState.ContainsKey("userEntity") && App.UserAccountEntity == null)
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
            _liveVm.BuildList();
            //CreateBackgroundTask();
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            string jsonObjectString = JsonConvert.SerializeObject(App.UserAccountEntity);
            e.PageState["userAccountEntity"] = jsonObjectString;
            jsonObjectString = JsonConvert.SerializeObject(App.UserAccountEntity.GetUserEntity());
            e.PageState["userEntity"] = jsonObjectString;
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
                    _vm.SetFriendsList(_user.OnlineId, true, false, false, true, true, false, true);
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
            var item = e.ClickedItem as MainPageViewModel.MessageGroupItem;
            if (item == null) return;
            string jsonObjectString = JsonConvert.SerializeObject(item.MessageGroup);
            Frame.Navigate(typeof (MessagePage), jsonObjectString);
        }

        private void FriendsListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FriendsEntity.Friend;
            if (item == null) return;
            Frame.Navigate(typeof (FriendPage), item.OnlineId);
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

        private void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFriendList();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (MainPivot.SelectedIndex)
            {
                case 0:
                    SetFriendList();
                    break;
                case 1:
                    _vm.SetMessages(_user.OnlineId, App.UserAccountEntity);
                    break;
                case 2:
                    _vm.SetInviteList();
                    break;
            }
        }

        private void LiveBroadcastGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var liveEntity = e.ClickedItem as LiveBroadcastEntity;
            if (liveEntity == null) return;
            var startUri = new Uri(liveEntity.Url);
            Launcher.LaunchUriAsync(startUri);
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (SearchPage));
        }

        private void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            _localSettings.Values["accessToken"] = string.Empty;
            _localSettings.Values["refreshToken"] = string.Empty;
            Frame.Navigate(typeof (LoginPage));
        }

        private void ProfileButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (FriendPage), _user.OnlineId);
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

        private async void AddFriendViaEmail_OnClick(object sender, RoutedEventArgs e)
        {
            GeneralProgressBar.Visibility = Visibility.Visible;
            var friendManager = new FriendManager();
            FriendTokenEntity tokenEntity = await friendManager.GetFriendLink(App.UserAccountEntity);
            if (tokenEntity == null)
            {
                GeneralProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            string subject = resourceLoader.GetString("AddFriendSubject/Text");
            var mail = new EmailMessage
            {
                Subject = subject,
                Body = string.Concat(subject, Environment.NewLine, tokenEntity.Token)
            };
            await EmailManager.ShowComposeNewEmailAsync(mail);
            GeneralProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void AddFriendViaSMS_OnClick(object sender, RoutedEventArgs e)
        {
            GeneralProgressBar.Visibility = Visibility.Visible;
            var friendManager = new FriendManager();
            FriendTokenEntity tokenEntity = await friendManager.GetFriendLink(App.UserAccountEntity);
            if (tokenEntity == null)
            {
                GeneralProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            string subject = resourceLoader.GetString("AddFriendSubject/Text");
            var chatMessage = new ChatMessage {Body = string.Concat(subject, Environment.NewLine, tokenEntity.Token)};
            await ChatMessageManager.ShowComposeSmsMessageAsync(chatMessage);
            GeneralProgressBar.Visibility = Visibility.Collapsed;
        }
    }
}