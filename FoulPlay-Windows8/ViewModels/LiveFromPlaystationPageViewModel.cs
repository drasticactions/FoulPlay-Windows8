using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;

namespace FoulPlay_Windows8.ViewModels
{
    public class LiveFromPlaystationPageViewModel : NotifierBase
    {

        private ObservableCollection<LiveBroadcastEntity> _liveBroadcastCollection;
        private readonly LiveStreamManager _liveStreamManager = new LiveStreamManager();
        public ObservableCollection<LiveBroadcastEntity> LiveBroadcastCollection
        {
            get { return _liveBroadcastCollection; }
            set
            {
                SetProperty(ref _liveBroadcastCollection, value);
                OnPropertyChanged();
            }
        }

        public LiveFromPlaystationPageViewModel()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
        }

        public void BuildList()
        {
            SetUstreamElements();
            SetTwitchElements();
        }

        private async void SetUstreamElements()
        {
            var filterList = new Dictionary<string, string> { { "platform", "PS4" }, { "type", "live" }, { "interactive", "true" } };
            var ustreamList = await _liveStreamManager.GetUstreamFeed(0, 80, "compact", filterList, "views", string.Empty, App.UserAccountEntity);
            if (ustreamList == null) return;
            if (ustreamList.items == null) return;
            foreach (var ustream in ustreamList.items)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromUstream(ustream);
                LiveBroadcastCollection.Add(entity);
            }
        }

        private async void SetTwitchElements()
        {
            var twitchList = await _liveStreamManager.GetTwitchFeed(0, 80, "PS4", "true", string.Empty, App.UserAccountEntity);
            if (twitchList == null) return;
            if (twitchList.streams == null) return;
            foreach (var twitch in twitchList.streams)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromTwitch(twitch);
                LiveBroadcastCollection.Add(entity);
            }
        }
    }
}
