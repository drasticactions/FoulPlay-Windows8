using System.Threading.Tasks;
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
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using FoulPlay_Windows8.UserControls;
using FoulPlay_Windows8.ViewModels;
using Newtonsoft.Json;

namespace FoulPlay_Windows8.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendPage : Page
    {
        private NavigationHelper navigationHelper;
        private string _userName;
        private FriendPageViewModel _vm;
        public FriendPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            FriendButtonStackPanel.Visibility = Visibility.Collapsed;
            _vm = (FriendPageViewModel)DataContext;
            if (e.PageState != null && e.PageState.ContainsKey("userEntity") && App.UserAccountEntity == null)
            {
                string savedStateJson = e.PageState["userAccountEntity"].ToString();
                App.UserAccountEntity = JsonConvert.DeserializeObject<UserAccountEntity>(savedStateJson);
                savedStateJson = e.PageState["userEntity"].ToString();
                var user = JsonConvert.DeserializeObject<UserAccountEntity.User>(savedStateJson);
                App.UserAccountEntity.SetUserEntity(user);
            }
            _userName = (string)e.NavigationParameter;
            _vm.SetRecentActivityFeed(_userName);
            _vm.SetFriendsList(_userName, false, false, false, false, true, false, false);
            _vm.SetTrophyList(_userName);
            _vm.SetMessages(_userName, App.UserAccountEntity);
            await _vm.SetUser(_userName);
            SetFriendButtons();
        }

        private async void SetFriendButtons()
        {
            var friend = _vm.UserModel.User;
            if (friend == null) return;
            if (friend.Relation.Equals("requested friend"))
            {
                var friendManager = new FriendManager();
                FriendMessage.Text = await friendManager.GetRequestMessage(_userName, App.UserAccountEntity);
                FriendButtonStackPanel.Visibility = Visibility.Visible;
            }
            if (friend.Relation.Equals("friend of friends") || friend.Relation.Equals("no relationship"))
            {
                FriendButtonStackPanel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            string jsonObjectString = JsonConvert.SerializeObject(App.UserAccountEntity);
            e.PageState["userAccountEntity"] = jsonObjectString;
            jsonObjectString = JsonConvert.SerializeObject(App.UserAccountEntity.GetUserEntity());
            e.PageState["userEntity"] = jsonObjectString;
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void FriendRequestButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.UserModel == null) return;
            var friend = _vm.UserModel.User;
            if (friend == null) return;
            if (friend.Relation.Equals("friend of friends") || friend.Relation.Equals("no relationship"))
            {
                
            }
        }

        private async void MessageSend_OnClick(object sender, RoutedEventArgs e)
        {
            MessageProgressBar.Visibility = Visibility.Visible;
            string messageId = string.Format("~{0},{1}", _userName, App.UserAccountEntity.GetUserEntity().OnlineId);
            var messageManager = new MessageManager();
            MessageSend.IsEnabled = false;
            ImageSend.IsEnabled = false;
            bool result;
            try
            {
                result =
                        await messageManager.CreatePost(messageId, MessageTextBox.Text, App.UserAccountEntity);
                MessageProgressBar.Visibility = Visibility.Collapsed;
                MessageSend.IsEnabled = true;
                ImageSend.IsEnabled = true;
            }
            catch (Exception)
            {
                result = false;
            }
            if (result)
            {
                MessageTextBox.Text = string.Empty;
                _vm.SetMessages(_userName, App.UserAccountEntity);
                return;
            }
            const string messageText = "An error has occured. The message has not been sent.";
            var msgDlg = new MessageDialog(messageText);
            await msgDlg.ShowAsync();
            MessageProgressBar.Visibility = Visibility.Collapsed;
            MessageSend.IsEnabled = true;
            ImageSend.IsEnabled = true;
        }

        private void ImageSend_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RefreshAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ActivityFeedListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RecentActivityEntity.Feed;
            if (item == null) return;
            var control = new RecentActivityUserControl();
            control.SetOffset();
            control.SetContext(item);
            control.OpenPopup();
        }

        private void TrophyListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as TrophyEntity.TrophyTitle;
            if (item == null) return;
            string jsonObjectString = JsonConvert.SerializeObject(item);
            Frame.Navigate(typeof(TrophyPage), jsonObjectString);
        }

        private void FriendsListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FriendsEntity.Friend;
            if (item == null) return;
            Frame.Navigate(typeof(FriendPage), item.OnlineId);
        }

        private async Task<BitmapImage> DecodeImage(Stream stream)
        {
            var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(memStream.AsRandomAccessStream());
            return bitmapImage;
        }

        private async void MessagesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            string messageId = string.Format("~{0},{1}", _userName, App.UserAccountEntity.GetUserEntity().OnlineId);
            MessageProgressBar.Visibility = Visibility.Visible;
            if (UserMessagePopup.IsOpen)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            UserMessageImage.Source = null;
            var messageItem = e.ClickedItem as FriendPageViewModel.MessageGroupItem;
            if (messageItem == null)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            MessageEntity.Message message = messageItem.Message;
            if (message == null)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            if (!message.contentKeys.Contains("image-data-0"))
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            try
            {
                var messageManager = new MessageManager();
                Stream imageBytes = await
                    messageManager.GetMessageContent(messageId, message,
                        App.UserAccountEntity);
                BitmapImage image = await DecodeImage(imageBytes);
                UserMessageImage.Source = image;
            }
            catch (Exception)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                UserMessageImage.Source = null;
                return;
            }
            MessageProgressBar.Visibility = Visibility.Collapsed;
            UserMessagePopup.HorizontalOffset = (Window.Current.Bounds.Width - 400) / 2;
            UserMessagePopup.VerticalOffset = (Window.Current.Bounds.Height - 600) / 2;
            UserMessagePopup.IsOpen = true;
        }

        private async void SendFriendRequest_OnClick(object sender, RoutedEventArgs e)
        {
            FriendRequestFlyout.Hide();
            var friendManager = new FriendManager();
            var result = await
                friendManager.SendFriendRequest(_userName, FriendRequesTextBox.Text, App.UserAccountEntity);
            if (!result)
            {
                const string messageText = "An error has occured. The friend request has not been sent.";
                var msgDlg = new MessageDialog(messageText);
                await msgDlg.ShowAsync();
                return;
            }
            await _vm.SetUser(_userName);
            SetFriendButtons();
        }

        private async void DeleteFriendRequest_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonPickerFlyout.Hide();
            var friendManager = new FriendManager();
            var result = await friendManager.DenyAddFriend(true, _userName, App.UserAccountEntity);
            if (!result)
            {
                const string messageText = "An error has occured. The friend request could not be removed.";
                var msgDlg = new MessageDialog(messageText);
                await msgDlg.ShowAsync();
                return;
            }
            await _vm.SetUser(_userName);
            SetFriendButtons();
        }

        private async void AddAsFriendButton_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonPickerFlyout.Hide();
            var friendManager = new FriendManager();
            var result = await friendManager.DenyAddFriend(false, _userName, App.UserAccountEntity);
            if (!result)
            {
                const string messageText = "An error has occured. Friend could not be added.";
                var msgDlg = new MessageDialog(messageText);
                await msgDlg.ShowAsync();
                return;
            }
            await _vm.SetUser(_userName);
            SetFriendButtons();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (MainPivot.SelectedIndex)
            {
                case 1:
                    _vm.SetMessages(_userName, App.UserAccountEntity);
                    break;
                case 2:
                    _vm.SetRecentActivityFeed(_userName);
                    break;
                case 3:
                    _vm.SetTrophyList(_userName);
                    break;
                case 4:
                    _vm.SetFriendsList(_userName, false, false, false, false, true, false, false);
                    break;
            }
        }
    }
}
