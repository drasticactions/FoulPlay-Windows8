using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FoulPlay_Windows8.Annotations;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;

namespace FoulPlay_Windows8.Tools
{
    public class InfiniteScrollingCollection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool HasMoreItems { get; protected set; }
        public string Username { get; set; }

        public int Offset;

        public bool OnlineFilter;

        public bool PersonalDetailSharing;

        public bool FriendStatus;

        public bool Requesting;

        public bool Requested;

        public bool RecentlyPlayed;

        public bool BlockedPlayer;

        public bool IsNews;

        public bool StorePromo;

        public UserAccountEntity UserAccountEntity;

        private bool _isLoading = false;

        public bool IsLoading
        {
            get
            {
                return _isLoading;

            }

            private set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<FriendsEntity.Friend> FriendList
        {
            get;
            set;
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void LoadFriends(string username)
        {
            IsLoading = true;
            var friendManager = new FriendManager();
            var friendEntity = await friendManager.GetFriendsList(username, Offset, BlockedPlayer, RecentlyPlayed, PersonalDetailSharing, FriendStatus, Requesting, Requested, OnlineFilter, UserAccountEntity);
            if (friendEntity == null)
            {
                //HasMoreItems = false;
                return;
            }
            foreach (var friend in friendEntity.FriendList)
            {
                FriendList.Add(friend);
            }
            if (friendEntity.FriendList.Any())
            {
                HasMoreItems = true;
                Offset = Offset += 32;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }
    }
}
