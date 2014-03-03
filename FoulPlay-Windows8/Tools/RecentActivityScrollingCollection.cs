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
using FoulPlay_Windows8.Annotations;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;

namespace FoulPlay_Windows8.Tools
{
    public class RecentActivityScrollingCollection : ObservableCollection<RecentActivityEntity.Feed>, ISupportIncrementalLoading
    {
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {

            if (!IsLoading)
            {
                await LoadFeedList(this.Username);
            }
            var ret = new LoadMoreItemsResult { Count = count };
            return ret;
        }

        public RecentActivityScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        public bool HasMoreItems { get; private set; }

        private bool _isLoading;
        public string Username { get; set; }
        public bool IsNews;
        public bool StorePromo;
        public UserAccountEntity UserAccountEntity;

        public int PageCount { get; set; }

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

        public async Task<bool> LoadFeedList(string username)
        {

            IsLoading = true;
            var recentActivityManager = new RecentActivityManager();
            var feedEntity =
                await recentActivityManager.GetActivityFeed(username, PageCount, StorePromo, IsNews, UserAccountEntity);
            if (feedEntity == null)
            {
                HasMoreItems = false;
                return false;
            }
            foreach (var feed in feedEntity.feed)
            {
                Add(feed);
            }
            if (feedEntity.feed.Any())
            {
                HasMoreItems = true;
                PageCount++;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
            return true;
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
    }
}
