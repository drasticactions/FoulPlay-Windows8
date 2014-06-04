using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using FoulPlay.Core.Entities;
using FoulPlay.Core.Managers;
using FoulPlay_Windows8.Annotations;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class SessionInviteScrollingCollection : ObservableCollection<SessionInviteEntity.Invitation>, ISupportIncrementalLoading
    {
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
            if (!IsLoading)
            {
                LoadInvites();
            }
            var ret = new LoadMoreItemsResult { Count = count };
            return ret;
        }

        public SessionInviteScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
        }

        public bool HasMoreItems { get; private set; }
        private bool _isEmpty;
        public UserAccountEntity UserAccountEntity;
        public int Offset;
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsLoading"));
            }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }

            private set
            {
                _isEmpty = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsEmpty"));
            }
        }

        public async void LoadInvites()
        {
            IsLoading = true;
            var sessionInviteManager = new SessionInviteManager();
            var inviteEntity = await sessionInviteManager.GetSessionInvites(Offset, UserAccountEntity);
            if (inviteEntity == null)
            {
                HasMoreItems = false;
                IsLoading = false;
                return;
            }
            if (inviteEntity.Invitations == null)
            {
                HasMoreItems = false;
                IsLoading = false;
                return;
            }
            foreach (var invite in inviteEntity.Invitations)
            {
                Add(invite);
            }
            if (inviteEntity.Invitations.Any())
            {
                // TODO: Only load once. HACK.
                HasMoreItems = false;
                Offset += 32;
            }
            else
            {
                IsEmpty = true;
                HasMoreItems = false;
            }
            IsLoading = false;
        }
    }
}
