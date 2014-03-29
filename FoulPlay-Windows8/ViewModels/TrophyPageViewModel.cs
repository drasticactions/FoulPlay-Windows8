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
    public class TrophyPageViewModel : NotifierBase
    {
        private TrophyDetailEntity _trophyDetailEntity;
        private ObservableCollection<TrophyDetailEntity.Trophy> _trophies;
        private readonly TrophyDetailManager _trophyDetailManager = new TrophyDetailManager();
        public TrophyPageViewModel()
        {
            _trophyDetailEntity = new TrophyDetailEntity();
            _trophies = new ObservableCollection<TrophyDetailEntity.Trophy>();
        }

        public ObservableCollection<TrophyDetailEntity.Trophy> Trophies
        {
            get { return _trophies; }
            set
            {
                SetProperty(ref _trophies, value);
                OnPropertyChanged();
            }
        }

        public async void SetTrophyList(string userName, string npCommunicationId)
        {
            TrophyDetailEntity trophys =
                await
                    _trophyDetailManager.GetTrophyDetailList(npCommunicationId,
                       userName, true,
                        App.UserAccountEntity);
            if (trophys == null) return;
            if (trophys.Trophies == null) return;
            foreach (var trophy in trophys.Trophies)
            {
                Trophies.Add(trophy);
            }
        }

    }
}
