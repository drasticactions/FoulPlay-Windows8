using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using FoulPlay_Windows8.Tools;

namespace FoulPlay_Windows8.ViewModels
{
    public class MainPageViewModel : NotifierBase
    {
        private FriendScrollingCollection _friendScrollingCollection;
        private RecentActivityScrollingCollection _recentActivityScrollingCollection;
        private MessageGroupEntity _messageGroupEntity;

        public MessageGroupEntity MessageGroupEntity
        {
            get { return _messageGroupEntity; }
            set
            {
                SetProperty(ref _messageGroupEntity, value);
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
                PageCount = 1
            };
        }

        public async void SetMessages(string userName, UserAccountEntity userAccountEntity)
        {
            var messageManager = new MessageManager();
            MessageGroupEntity = await messageManager.GetMessageGroup(userName, userAccountEntity);
        }
    }
}
