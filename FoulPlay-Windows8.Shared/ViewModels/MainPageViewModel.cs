using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
using FoulPlay.Core.Tools;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using FoulPlay_Windows8.Tools;

namespace FoulPlay_Windows8.ViewModels
{
    public class MainPageViewModel : NotifierBase
    {
        private FriendScrollingCollection _friendScrollingCollection;

        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();

        private ObservableCollection<MessageGroupItem> _messageGroupCollection =
            new ObservableCollection<MessageGroupItem>();

        private bool _messageGroupEmpty;
        private MessageGroupEntity _messageGroupEntity;
        private bool _messageGroupLoading;
        private RecentActivityScrollingCollection _recentActivityScrollingCollection;
        private SessionInviteScrollingCollection _sessionInviteScrollingCollection;

        public bool MessageGroupEmpty
        {
            get { return _messageGroupEmpty; }
            set
            {
                SetProperty(ref _messageGroupEmpty, value);
                OnPropertyChanged();
            }
        }

        public bool MessageGroupLoading
        {
            get { return _messageGroupLoading; }
            set
            {
                SetProperty(ref _messageGroupLoading, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set
            {
                SetProperty(ref _menuItems, value);
                OnPropertyChanged();
            }
        }

        public SessionInviteScrollingCollection SessionInviteScrollingCollection
        {
            get { return _sessionInviteScrollingCollection; }
            set
            {
                SetProperty(ref _sessionInviteScrollingCollection, value);
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

        public void SetInviteList()
        {
            SessionInviteScrollingCollection = new SessionInviteScrollingCollection
            {
                Offset = 0,
                UserAccountEntity = App.UserAccountEntity
            };
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

        public async void SetRecentActivityFeed(string userName)
        {
            RecentActivityScrollingCollection = new RecentActivityScrollingCollection
            {
                IsNews = true,
                StorePromo = true,
                UserAccountEntity = App.UserAccountEntity,
                Username = userName,
                PageCount = 0
            };
        }

        public async void SetMessages(string userName, UserAccountEntity userAccountEntity)
        {
            MessageGroupLoading = true;
            MessageGroupCollection = new ObservableCollection<MessageGroupItem>();
            var messageManager = new MessageManager();
            _messageGroupEntity = await messageManager.GetMessageGroup(userName, userAccountEntity);

            foreach (
                MessageGroupItem newMessage in
                    _messageGroupEntity.MessageGroups.Select(message => new MessageGroupItem {MessageGroup = message}))
            {
                GetAvatar(newMessage, userAccountEntity);
                MessageGroupCollection.Add(newMessage);
            }
            if (MessageGroupCollection.Count <= 0)
            {
                MessageGroupEmpty = true;
            }
            MessageGroupLoading = false;
        }

        private async void GetAvatar(MessageGroupItem message, UserAccountEntity userAccountEntity)
        {
            UserEntity user =
                await UserManager.GetUserAvatar(message.MessageGroup.LatestMessage.SenderOnlineId, userAccountEntity);
            if (user == null) return;
            message.AvatarUrl = user.AvatarUrl;
            OnPropertyChanged("MessageGroupCollection");
        }

        public void CreateMenu()
        {
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            MenuItems.Add(new MenuItem("/Assets/phone_home_footerIcon_region.png",
                resourceLoader.GetString("RecentActivity/Text"), "recent"));
            MenuItems.Add(new MenuItem("/Assets/appbar.film.png", resourceLoader.GetString("LiveFromPlaystation/Text"),
                "live"));
            MenuItems.Add(new MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
                resourceLoader.GetString("ProfileHeader/Text"), "profile"));
        }

        public void CreateMenuPhone()
        {
            if (MenuItems.Any()) return;
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            MenuItems.Add(new MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
                resourceLoader.GetString("FriendsPivot/Text"), "friends"));
            MenuItems.Add(new MenuItem("/Assets/phone_home_footerIcon_region.png",
                resourceLoader.GetString("RecentActivity/Text"), "recent"));
            MenuItems.Add(new MenuItem("/Assets/appbar.film.png", resourceLoader.GetString("LiveFromPlaystation/Text"),
                "live"));
            MenuItems.Add(new MenuItem("/Assets/phone_trophy_icon_compareTrophies.png",
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
    }
}