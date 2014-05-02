using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoulPlay.Core.Entities;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;

namespace FoulPlay_Windows8.ViewModels
{
    public class LiveFromPlaystationPageViewModel : NotifierBase
    {
        private readonly LiveStreamManager _liveStreamManager = new LiveStreamManager();
        private ObservableCollection<LiveBroadcastEntity> _liveBroadcastCollection;
        private bool _isLoading;
        private bool _isEmpty;

        public LiveFromPlaystationPageViewModel()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
        }

        public ObservableCollection<LiveBroadcastEntity> LiveBroadcastCollection
        {
            get { return _liveBroadcastCollection; }
            set
            {
                SetProperty(ref _liveBroadcastCollection, value);
                OnPropertyChanged();
            }
        }


        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged();
            }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
            set
            {
                SetProperty(ref _isEmpty, value);
                OnPropertyChanged();
            }
        }

        public void BuildList()
        {
            SetUstreamElements();
            SetTwitchElements();
            SetNicoDougaElements();
        }

        private async void SetUstreamElements()
        {
            IsLoading = true;
            var filterList = new Dictionary<string, string>
            {
                {"platform", "PS4"},
                {"type", "live"},
                {"interactive", "true"}
            };
            UstreamEntity ustreamList =
                await
                    _liveStreamManager.GetUstreamFeed(0, 80, "compact", filterList, "views", string.Empty,
                        App.UserAccountEntity);
            if (ustreamList == null) return;
            if (ustreamList.items == null) return;
            foreach (UstreamEntity.Item ustream in ustreamList.items)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromUstream(ustream);
                LiveBroadcastCollection.Add(entity);
            }
            IsLoading = false;

        }

        private async void SetTwitchElements()
        {
            IsLoading = true;
            TwitchEntity twitchList =
                await _liveStreamManager.GetTwitchFeed(0, 80, "PS4", "true", string.Empty, App.UserAccountEntity);
            if (twitchList == null) return;
            if (twitchList.streams == null) return;
            foreach (TwitchEntity.Stream twitch in twitchList.streams)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromTwitch(twitch);
                LiveBroadcastCollection.Add(entity);
            }
            IsLoading = false;

        }

        private async void SetNicoDougaElements()
        {
            IsLoading = true;

            NicoNicoEntity nicoNicoEntity = await _liveStreamManager.GetNicoFeed("onair", "PS4", 0, 80, "view", App.UserAccountEntity);
            if (nicoNicoEntity == null) return;
            if (nicoNicoEntity.programs == null) return;
            foreach (NicoNicoEntity.Program program in nicoNicoEntity.programs)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromNicoNico(program);
                LiveBroadcastCollection.Add(entity);
            }

            IsLoading = false;
        }
    }
}