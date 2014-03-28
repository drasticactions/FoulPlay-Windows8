using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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
using FoulPlay_Windows8.ViewModels;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace FoulPlay_Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FriendPage : Page
    {
        private FriendPageViewModel _vm;
        private readonly NavigationHelper navigationHelper;
        private string _userName;
        public StorageFile File { get; private set; }


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
            _userName = (string)e.NavigationParameter;
            _vm.SetUser(_userName);
            _vm.SetRecentActivityFeed(_userName);
            _vm.SetFriendsList(_userName, false, false, false, false, true, false, false);
            _vm.SetTrophyList(_userName);
            _vm.SetMessages(_userName, App.UserAccountEntity);
            //_user = JsonConvert.DeserializeObject<UserEntity>(jsonObjectString);
            //var isCurrentUser = App.UserAccountEntity.GetUserEntity().OnlineId.Equals(_user.OnlineId);
            //if (isCurrentUser)
            //{
            //    MessagesGrid.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    RefreshGroupMessages();
            //}

            //var languageList = _user.LanguagesUsed.Select(ParseLanguageVariable).ToList();
            //MyLanguagesBlock.Text = string.Join("," + Environment.NewLine, languageList);
            //UserInformationGrid.DataContext = _user;
            //UserInformationHeaderGrid.DataContext = _user;
            //LoadRecentActivityList();
            //GetTrophyList();
            //GetFriendsList(false, false, false, false, true, false, false);
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
            _vm = (FriendPageViewModel)DataContext;
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

        private async void FriendsListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FriendsEntity.Friend;
            if (item == null) return;
            Frame.Navigate(typeof(FriendPage), item.OnlineId);
        }

        private async void MessageSend_OnClick(object sender, RoutedEventArgs e)
        {
            MessageProgressBar.Visibility = Visibility.Visible;
            string messageId = string.Format("~{0},{1}", _userName, App.UserAccountEntity.GetUserEntity().OnlineId);
            var messageManager = new MessageManager();
            MessageSend.IsEnabled = false;
            ImageSend.IsEnabled = false;
            //CameraAccess.IsEnabled = false;
            bool result = false;
            if (ImageSource.Source != null)
            {
                using (IRandomAccessStream stream = await File.OpenAsync(FileAccessMode.ReadWrite))
                {
                   
                    // TODO: Add JPEG Compression
                    //result = await
                    //messageManager.CreatePostWithMedia(messageId, MessageTextBox.Text, "", pixels,
                    //    App.UserAccountEntity);
                }
                
            }
            else
            {
                result = await messageManager.CreatePost(messageId, MessageTextBox.Text, App.UserAccountEntity);
            }
            MessageProgressBar.Visibility = Visibility.Collapsed;
            MessageSend.IsEnabled = true;
            ImageSend.IsEnabled = true;
            if (result)
            {
                ImageSource.Source = null;
                MessageTextBox.Text = string.Empty;
                //RefreshGroupMessages();
                return;
            }
            const string messageText = "An error has occured. The message has not been sent.";
                var msgDlg = new MessageDialog(messageText);
                await msgDlg.ShowAsync();
        }

        private async void ImageSend_OnClick(object sender, RoutedEventArgs e)
        {
            MessageSend.IsEnabled = false;
            ImageSend.IsEnabled = false;

            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            var bitmapimage = new BitmapImage();
            File = await openPicker.PickSingleFileAsync();
            if (File != null)
            {
                IRandomAccessStream stream = await File.OpenAsync(FileAccessMode.Read);
                await bitmapimage.SetSourceAsync(stream);
                ImageSource.Source = bitmapimage;
            }
            MessageSend.IsEnabled = true;
            ImageSend.IsEnabled = true;
        }
    }
}
