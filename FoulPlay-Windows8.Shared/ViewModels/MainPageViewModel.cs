using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using FoulPlay_Windows8.Tools;

namespace FoulPlay_Windows8.ViewModels
{
    public class MainPageViewModel : NotifierBase
    {
        private FriendScrollingCollection _friendScrollingCollection;

        private ObservableCollection<MessageGroupItem> _messageGroupCollection =
            new ObservableCollection<MessageGroupItem>();

        private MessageGroupEntity _messageGroupEntity;
        private RecentActivityScrollingCollection _recentActivityScrollingCollection;

        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set
            {
                SetProperty(ref _menuItems, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MessageGroupItem> MessageGroupCollection
        {
            get { return _messageGroupCollection; }
            set
            {
                SetProperty(ref _messageGroupCollection, value);
                OnPropertyChanged();
            }
        }

        public FriendScrollingCollection FriendScrollingCollection
        {
            get { return _friendScrollingCollection; }
            set
            {
                SetProperty(ref _friendScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public RecentActivityScrollingCollection RecentActivityScrollingCollection
        {
            get { return _recentActivityScrollingCollection; }
            set
            {
                SetProperty(ref _recentActivityScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public void SetFriendsList(string userName, bool onlineFilter, bool blockedPlayer, bool recentlyPlayed,
            bool personalDetailSharing, bool friendStatus, bool requesting, bool requested)
        {
            FriendScrollingCollection = new FriendScrollingCollection
            {
                UserAccountEntity = App.UserAccountEntity,
                Offset = 0,
                OnlineFilter = onlineFilter,
                Requested = requested,
                Requesting = requesting,
                PersonalDetailSharing = personalDetailSharing,
                FriendStatus = friendStatus,
                Username = userName
            };
        }

        public void SetRecentActivityFeed(string userName)
        {
            RecentActivityScrollingCollection = new RecentActivityScrollingCollection
            {
                IsNews = true,
                StorePromo = true,
                UserAccountEntity = App.UserAccountEntity,
                Username = userName,
                PageCount = 0
            };
            RecentActivityScrollingCollection.LoadFeedList(userName);
        }

        public async void SetMessages(string userName, UserAccountEntity userAccountEntity)
        {
            MessageGroupCollection = new ObservableCollection<MessageGroupItem>();
            var messageManager = new MessageManager();
            _messageGroupEntity = await messageManager.GetMessageGroup(userName, userAccountEntity);

            foreach (MessageGroupEntity.MessageGroup message in _messageGroupEntity.MessageGroups)
            {
                var newMessage = new MessageGroupItem {MessageGroup = message};
                GetAvatar(newMessage, userAccountEntity);
                MessageGroupCollection.Add(newMessage);
            }
        }

        private async void GetAvatar(MessageGroupItem message, UserAccountEntity userAccountEntity)
        {
            UserEntity user =
                await UserManager.GetUserAvatar(message.MessageGroup.LatestMessage.SenderOnlineId, userAccountEntity);
            message.AvatarUrl = user.AvatarUrl;
            OnPropertyChanged("MessageGroupCollection");
        }

        /// <summary>
        ///     TODO: Seperate to new class, use ISupportIncrementalLoading
        /// </summary>
        public class MessageGroupItem : NotifierBase
        {
            private string _avatarUrl;

            public string AvatarUrl
            {
                get { return _avatarUrl; }
                set
                {
                    SetProperty(ref _avatarUrl, value);
                    OnPropertyChanged();
                }
            }

            public MessageGroupEntity.MessageGroup MessageGroup { get; set; }
        }

        // TODO: Change for Windows Phone 8.1
        public void CreateMenu()
        {
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            MenuItems.Add(new MainPageViewModel.MenuItem("/Assets/phone_home_footerIcon_region.png",
                resourceLoader.GetString("RecentActivity/Text"), "recent"));
            MenuItems.Add(new MainPageViewModel.MenuItem("/Assets/appbar.film.png", resourceLoader.GetString("LiveFromPlaystation/Text"),
                "live"));
            MenuItems.Add(new MainPageViewModel.MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
                resourceLoader.GetString("ProfileHeader/Text"), "profile"));
            MenuItems.Add(new MainPageViewModel.MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
    resourceLoader.GetString("ProfileHeader/Text"), "profile"));
            MenuItems.Add(new MainPageViewModel.MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
    resourceLoader.GetString("ProfileHeader/Text"), "profile"));
            MenuItems.Add(new MainPageViewModel.MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
    resourceLoader.GetString("ProfileHeader/Text"), "profile"));
            MenuItems.Add(new MainPageViewModel.MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
    resourceLoader.GetString("ProfileHeader/Text"), "profile"));
        }

        public class MenuItem
        {
            public MenuItem(string icon, string text, string location)
            {
                Text = text;
                Icon = icon;
                Location = location;
            }

            public string Text { get; private set; }

            public string Location { get; private set; }

            public string Icon { get; private set; }
        }
    }
}