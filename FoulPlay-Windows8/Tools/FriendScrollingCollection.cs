using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using FoulPlay_Windows8.Annotations;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using FoulPlay_Windows8.Views;

namespace FoulPlay_Windows8.Tools
{
    public class FriendScrollingCollection : ObservableCollection<FriendsEntity.Friend>, ISupportIncrementalLoading
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }
        public FriendScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
            UserAccountEntity = App.UserAccountEntity;
        }
        private async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {

                    if (!IsLoading)
                    {
                        await LoadFriends(this.Username);
                    }
            var ret = new LoadMoreItemsResult { Count = count };
            return ret;
        }

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<bool> LoadFriends(string username)
        {
            IsLoading = true;
            var friendManager = new FriendManager();
            var friendEntity = await friendManager.GetFriendsList(username, Offset, BlockedPlayer, RecentlyPlayed, PersonalDetailSharing, FriendStatus, Requesting, Requested, OnlineFilter, UserAccountEntity);
            if (friendEntity == null)
            {
                return false;
            }
            foreach (var friend in friendEntity.FriendList)
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
            }
            IsLoading = false;
            return true;
        }
    }
}
