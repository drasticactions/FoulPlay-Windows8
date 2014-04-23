using System.Threading.Tasks;
using Windows.Storage;
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
using FoulPlay_Windows8.ViewModels;
using Newtonsoft.Json;

namespace FoulPlay_Windows8.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessagePage : Page
    {
        private NavigationHelper navigationHelper;
        private MessageGroupEntity.MessageGroup _messageGroup;
        private UserAccountEntity.User _user;
        private MessagePageViewModel _vm;

        public MessagePage()
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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _vm = (MessagePageViewModel)DataContext;

            if (e.PageState != null && e.PageState.ContainsKey("userEntity") && App.UserAccountEntity == null)
            {
                string savedStateJson = e.PageState["userAccountEntity"].ToString();
                App.UserAccountEntity = JsonConvert.DeserializeObject<UserAccountEntity>(savedStateJson);
                savedStateJson = e.PageState["userEntity"].ToString();
                _user = JsonConvert.DeserializeObject<UserAccountEntity.User>(savedStateJson);
                App.UserAccountEntity.SetUserEntity(_user);
            }
            var jsonObjectString = (string)e.NavigationParameter;
            _messageGroup = JsonConvert.DeserializeObject<MessageGroupEntity.MessageGroup>(jsonObjectString);
            _vm.SetMessages(_messageGroup.MessageGroupId, App.UserAccountEntity);
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

        private void ImageSend_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void MessageSend_OnClick(object sender, RoutedEventArgs e)
        {
            MessageProgressBar.Visibility = Visibility.Visible;
            var messageManager = new MessageManager();
            MessageSend.IsEnabled = false;
            ImageSend.IsEnabled = false;
            bool result;
            try
            {
                    result =
                        await
                            messageManager.CreatePost(_messageGroup.MessageGroupId, MessageTextBox.Text,
                                App.UserAccountEntity);
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
                _vm.SetMessages(_messageGroup.MessageGroupId, App.UserAccountEntity);
                return;
            }
            const string messageText = "An error has occured. The message has not been sent.";
            var msgDlg = new MessageDialog(messageText);
            await msgDlg.ShowAsync();
            MessageProgressBar.Visibility = Visibility.Collapsed;
            MessageSend.IsEnabled = true;
            ImageSend.IsEnabled = true;
        }

        private async void MessagesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            MessageProgressBar.Visibility = Visibility.Visible;
            if (UserMessagePopup.IsOpen)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            UserImage.Source = null;
            var messageItem = e.ClickedItem as MessagePageViewModel.MessageGroupItem;
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
                    messageManager.GetMessageContent(_messageGroup.MessageGroupId, message,
                        App.UserAccountEntity);
                BitmapImage image = await DecodeImage(imageBytes);
                UserImage.Source = image;
            }
            catch (Exception)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                UserImage.Source = null;
                return;
            }
            MessageProgressBar.Visibility = Visibility.Collapsed;
            UserMessagePopup.HorizontalOffset = (Window.Current.Bounds.Width - 400) / 2;
            UserMessagePopup.VerticalOffset = (Window.Current.Bounds.Height - 600) / 2;
            UserMessagePopup.IsOpen = true;
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
    }
}
