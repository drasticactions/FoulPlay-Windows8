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

        public new event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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
            UserAccountEntity = App.UserAccountEntity;
        }

        public bool HasMoreItems { get; private set; }
        public UserAccountEntity UserAccountEntity;
        public int Offset;
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                OnPropertyChanged();
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
                return;
            }
            if (inviteEntity.Invitations == null)
            {
                HasMoreItems = false;
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
                HasMoreItems = false;
            }
            IsLoading = false;
        }
    }
}
