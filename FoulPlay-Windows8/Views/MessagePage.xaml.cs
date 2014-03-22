using System.Threading.Tasks;
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
using Newtonsoft.Json;

namespace FoulPlay_Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MessagePage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private UserAccountEntity.User _user;
        private MessageGroupEntity.MessageGroup _messageGroup;
        public StorageFile File { get; private set; }

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


        public MessagePage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        private async void RefreshGroupMessages()
        {
            MessageProgressBar.Visibility = Visibility.Visible;
            var messagerManager = new MessageManager();
            var messageEntity = await messagerManager.GetGroupConversation(_messageGroup.MessageGroupId, App.UserAccountEntity);
            if (messageEntity == null)
            {
                MessageProgressBar.Visibility = Visibility.Collapsed;
                MessageSend.IsEnabled = true;
                ImageSend.IsEnabled = true;
                return;
            }
            MessagesListView.DataContext = messageEntity;
            await messagerManager.ClearMessages(messageEntity, App.UserAccountEntity);
            MessageProgressBar.Visibility = Visibility.Collapsed;
            MessageSend.IsEnabled = true;
            ImageSend.IsEnabled = true;
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
                _user = JsonConvert.DeserializeObject<UserAccountEntity.User>(savedStateJson);
                App.UserAccountEntity.SetUserEntity(_user);
            }
            var jsonObjectString = (string)e.NavigationParameter;
            _messageGroup = JsonConvert.DeserializeObject<MessageGroupEntity.MessageGroup>(jsonObjectString);
            RefreshGroupMessages();
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

        private async Task<byte[]> ImageToBytes(IRandomAccessStream sourceStream)
        {
            byte[] imageArray;

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);

            var transform = new BitmapTransform { ScaledWidth = decoder.PixelWidth, ScaledHeight = decoder.PixelHeight };
            PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Rgba8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.RespectExifOrientation,
                ColorManagementMode.DoNotColorManage);

            using (var destinationStream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destinationStream);
                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, decoder.PixelWidth,
                                        decoder.PixelHeight, 96, 96, pixelData.DetachPixelData());
                await encoder.FlushAsync();

                BitmapDecoder outputDecoder = await BitmapDecoder.CreateAsync(destinationStream);
                await destinationStream.FlushAsync();
                imageArray = (await outputDecoder.GetPixelDataAsync()).DetachPixelData();
            }
            return imageArray;
        }

        private async void MessageSend_OnClick(object sender, RoutedEventArgs e)
        {
            MessageProgressBar.Visibility = Visibility.Visible;
            var messageManager = new MessageManager();
            MessageSend.IsEnabled = false;
            ImageSend.IsEnabled = false;
            bool result = false;
            try
            {
                if (ImageSource.Source != null)
                {
                    using (IRandomAccessStream stream = await File.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var byteArray = await ImageToBytes(stream);
                        await messageManager.CreatePostWithMedia(_messageGroup.MessageGroupId, MessageTextBox.Text, "", byteArray,
                           App.UserAccountEntity);
                    }
                }
                else
                {
                    result = await messageManager.CreatePost(_messageGroup.MessageGroupId, MessageTextBox.Text, App.UserAccountEntity);
                }
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
                ImageSource.Source = null;
                MessageTextBox.Text = string.Empty;
                RefreshGroupMessages();
                return;
            }
            const string messageText = "An error has occured. The message has not been sent.";
            var msgDlg = new MessageDialog(messageText);
            await msgDlg.ShowAsync();
            MessageProgressBar.Visibility = Visibility.Collapsed;
            MessageSend.IsEnabled = true;
            ImageSend.IsEnabled = true;
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

        private async void MessagesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var message = e.ClickedItem as MessageEntity.Message;
            if (message == null) return;
            UserMessageGrid.DataContext = message;
            if (message.contentKeys == null)
            {
                MessageImageGrid.Visibility = Visibility.Collapsed;
                return;
            }
            if (!message.contentKeys.Contains("image-data-0"))
            {
                MessageImageGrid.Visibility = Visibility.Collapsed;
                return;
            }
            MessageImageGrid.Visibility = Visibility.Visible;
            MessageImage.Visibility = Visibility.Collapsed;
            LoadingProgressRing.Visibility = Visibility.Visible;
            try
            {
                var messageManager = new MessageManager();
                var imageBytes = await
                       messageManager.GetMessageContent(_messageGroup.MessageGroupId, message,
                           App.UserAccountEntity);
                var image = await DecodeImage(imageBytes);
                MessageImage.Source = image;
            }
            catch (Exception)
            {
                MessageImageGrid.Visibility = Visibility.Collapsed;
            }
            LoadingProgressRing.Visibility = Visibility.Collapsed;
            MessageImage.Visibility = Visibility.Visible;
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
