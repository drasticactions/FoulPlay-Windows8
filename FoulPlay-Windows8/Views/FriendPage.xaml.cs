using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
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
    public sealed partial class FriendPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private UserEntity _user;
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


        public FriendPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
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
                var savedStateJson = e.PageState["userAccountEntity"].ToString();
                App.UserAccountEntity = JsonConvert.DeserializeObject<UserAccountEntity>(savedStateJson);
                savedStateJson = e.PageState["userEntity"].ToString();
                var user = JsonConvert.DeserializeObject<UserAccountEntity.User>(savedStateJson);
                App.UserAccountEntity.SetUserEntity(user);
            }
            var jsonObjectString = (string)e.NavigationParameter;
            _user = JsonConvert.DeserializeObject<UserEntity>(jsonObjectString);
            var isCurrentUser = App.UserAccountEntity.GetUserEntity().OnlineId.Equals(_user.OnlineId);
            if (isCurrentUser)
            {
                MessagesGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                RefreshGroupMessages();
            }

            var languageList = _user.LanguagesUsed.Select(ParseLanguageVariable).ToList();
            MyLanguagesBlock.Text = string.Join("," + Environment.NewLine, languageList);
            UserInformationGrid.DataContext = _user;
            UserInformationHeaderGrid.DataContext = _user;
            LoadRecentActivityList();
            GetTrophyList();
            GetFriendsList(false, false, false, false, true, false, false);
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

        private MessageEntity _messageEntity;

        private async void RefreshGroupMessages()
        {
            MessageProgressBar.Visibility = Visibility.Visible;
            var messagerManager = new MessageManager();
            _messageEntity = await messagerManager.GetGroupConversation(string.Format("~{0},{1}", _user.OnlineId, App.UserAccountEntity.GetUserEntity().OnlineId), App.UserAccountEntity);
            if (_messageEntity == null)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                return;
            }

            if (_messageEntity.messages == null)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                return;
            }

            MessagesListView.DataContext = _messageEntity;
            await messagerManager.ClearMessages(_messageEntity, App.UserAccountEntity);
            MessageProgressBar.Visibility = Visibility.Collapsed;
            MessageSend.IsEnabled = true;
        }

        private async void LoadRecentActivityList()
        {
            //LoadingProgressBar.Visibility = Visibility.Visible;
            var recentActivityCollection = new RecentActivityScrollingCollection
            {
                IsNews = false,
                StorePromo = false,
                UserAccountEntity = App.UserAccountEntity,
                Username = _user.OnlineId,
                PageCount = 1
            };
            var recentActivityManager = new RecentActivityManager();
            var recentActivityEntity =
                await recentActivityManager.GetActivityFeed(_user.OnlineId, 0, false, false, App.UserAccountEntity);
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

        private async void GetTrophyList()
        {
            var trophyManager = new TrophyManager();
            var trophyCollection = new TrophyScrollingCollection()
            {
                UserAccountEntity = App.UserAccountEntity,
                Username = _user.OnlineId,
                Offset = 64
            };
            var items = await trophyManager.GetTrophyList(_user.OnlineId, 0, App.UserAccountEntity);
            if (items == null) return;
            foreach (TrophyEntity.TrophyTitle item in items.TrophyTitles)
            {
                trophyCollection.Add(item);
            }
            if (!items.TrophyTitles.Any())
            {
                //NoTrophyTextBlock.Visibility = Visibility.Visible;
                //TrophyHeaderGrid.Visibility = Visibility.Collapsed;
            }
            DefaultViewModel["TrophyList"] = trophyCollection;
            ComparedUserNameBlock.Text = App.UserAccountEntity.GetUserEntity().OnlineId.Equals(_user.OnlineId) ? string.Empty : _user.OnlineId;
            FromUserNameBlock.Text = App.UserAccountEntity.GetUserEntity().OnlineId;
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

        private void TrophyListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as TrophyEntity.TrophyTitle;
            if (item == null) return;
            string jsonObjectString = JsonConvert.SerializeObject(item);
            Frame.Navigate(typeof(TrophyPage), jsonObjectString);
        }

        private static string ParseLanguageVariable(string language)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView(); 
            switch (language)
            {
                case "ja":
                    return resourceLoader.GetString("LangJapanese/Text").Trim();
                case "dk":
                    return resourceLoader.GetString("LangDanish/Text").Trim();
                case "de":
                    return resourceLoader.GetString("LangGerman/Text").Trim();
                case "en":
                    return resourceLoader.GetString("LangEnglishUS/Text").Trim();
                case "en-GB":
                    return resourceLoader.GetString("LangEnglishUK/Text").Trim();
                case "fi":
                    return resourceLoader.GetString("LangFinnish/Text").Trim();
                case "fr":
                    return resourceLoader.GetString("LangFrench/Text").Trim();
                case "es":
                    return resourceLoader.GetString("LangSpanishSpain/Text").Trim();
                case "es-MX":
                    return resourceLoader.GetString("LangSpanishLA/Text").Trim();
                case "it":
                    return resourceLoader.GetString("LangItalian/Text").Trim();
                case "nl":
                    return resourceLoader.GetString("LangDutch/Text").Trim();
                case "pt":
                    return resourceLoader.GetString("LangPortuguesePortugal/Text").Trim();
                case "pt-BR":
                    return resourceLoader.GetString("LangPortugueseBrazil/Text").Trim();
                case "ru":
                    return resourceLoader.GetString("LangRussian/Text").Trim();
                case "pl":
                    return resourceLoader.GetString("LangPolish/Text").Trim();
                case "no":
                    return resourceLoader.GetString("LangNorwegian/Text").Trim();
                case "sv":
                    return resourceLoader.GetString("LangSwedish/Text").Trim();
                case "tr":
                    return resourceLoader.GetString("LangTurkish/Text").Trim();
                case "ko":
                    return resourceLoader.GetString("LangKorean/Text").Trim();
                case "zh-CN":
                    return resourceLoader.GetString("LangChineseSimplified/Text").Trim();
                case "zh-TW":
                    return resourceLoader.GetString("LangChineseTraditional/Text").Trim();
                default:
                    return null;
            }
        }

        public async void GetFriendsList(bool onlineFilter, bool blockedPlayer, bool recentlyPlayed,
            bool personalDetailSharing, bool friendStatus, bool requesting, bool requested)
        {
            var friendManager = new FriendManager();
            var friendCollection = new FriendScrollingCollection
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
                FriendsProgressBar.Visibility = Visibility.Collapsed;
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
                friendCollection.Add(item);
            }
            //FriendsLongListSelector.ItemRealized += friendList_ItemRealized;
            DefaultViewModel["FriendList"] = friendCollection;
            FriendsProgressBar.Visibility = Visibility.Collapsed;
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
