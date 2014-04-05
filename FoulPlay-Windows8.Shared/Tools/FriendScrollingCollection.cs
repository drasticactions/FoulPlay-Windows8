using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using FoulPlay_Windows8.Annotations;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;

namespace FoulPlay_Windows8.Tools
{
    public class FriendScrollingCollection : ObservableCollection<FriendsEntity.Friend>, ISupportIncrementalLoading, INotifyPropertyChanged
    {
        public bool BlockedPlayer;
        public bool FriendStatus;
        public int Offset;

        public bool OnlineFilter;

        public bool PersonalDetailSharing;

        public bool RecentlyPlayed;
        public bool Requested;
        public bool Requesting;
        private bool _isEmpty;
        public UserAccountEntity UserAccountEntity;

        private bool _isLoading;

        public FriendScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
            UserAccountEntity = App.UserAccountEntity;
        }

        public string Username { get; set; }

        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }

            private set
            {
                _isEmpty = value;
                OnPropertyChanged();
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        public bool HasMoreItems { get; protected set; }
        public new event PropertyChangedEventHandler PropertyChanged;

        private async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
            if (!IsLoading)
            {
                LoadFriends(Username);
            }
            var ret = new LoadMoreItemsResult {Count = count};
            return ret;
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
            FriendsEntity friendEntity =
                await
                    friendManager.GetFriendsList(username, Offset, BlockedPlayer, RecentlyPlayed, PersonalDetailSharing,
                        FriendStatus, Requesting, Requested, OnlineFilter, UserAccountEntity);
            if (friendEntity == null)
            {
                return;
            }
            if (friendEntity.FriendList == null)
            {
                HasMoreItems = false;
                return;
            }
            foreach (FriendsEntity.Friend friend in friendEntity.FriendList)
            {
                Add(friend);
            }
            if (friendEntity.FriendList.Any())
            {
                HasMoreItems = true;
                Offset = Offset += 32;
            }
            else
            {
                HasMoreItems = false;
                if (Count <= 0)
                {
                    IsEmpty = true;
                }
            }
            IsLoading = false;
        }
    }
}