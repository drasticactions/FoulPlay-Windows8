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
            var jsonObjectString = (string)e.NavigationParameter;
            _user = JsonConvert.DeserializeObject<UserEntity>(jsonObjectString);
            UserInformationGrid.DataContext = _user;
            UserInformationHeaderGrid.DataContext = _user;
            LoadRecentActivityList();
            GetTrophyList();
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
            ComparedUserNameBlock.Text = _user.OnlineId;
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
    }
}
