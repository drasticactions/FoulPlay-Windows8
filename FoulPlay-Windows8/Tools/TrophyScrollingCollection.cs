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
    public class TrophyScrollingCollection : ObservableCollection<TrophyEntity.TrophyTitle>, ISupportIncrementalLoading
    {
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
            if (!IsLoading)
            {
                await LoadTrophies(this.Username);
            }
            var ret = new LoadMoreItemsResult { Count = count };
            return ret;
        }

        public TrophyScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
        }

        public bool HasMoreItems { get; private set; }
        public string Username { get; set; }
        public int Offset;
        public int MaxCount { get; set; }
        public new event PropertyChangedEventHandler PropertyChanged;
        public UserAccountEntity UserAccountEntity;
        private bool _isLoading;
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

        public async Task<bool> LoadTrophies(string username)
        {
            Offset = Offset + MaxCount;
            IsLoading = true;
            var trophyManager = new TrophyManager();
            var trophyList = await trophyManager.GetTrophyList(username, Offset, UserAccountEntity);
            if (trophyList == null)
            {
                //HasMoreItems = false;
                return false;
            }
            foreach (var trophy in trophyList.TrophyTitles)
            {
                Add(trophy);
            }
            if (trophyList.TrophyTitles.Any())
            {
                HasMoreItems = true;
                MaxCount += 64;
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
