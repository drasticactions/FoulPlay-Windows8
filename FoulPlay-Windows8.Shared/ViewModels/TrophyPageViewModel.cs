﻿using System.Collections.ObjectModel;
using System.Linq;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;

namespace FoulPlay_Windows8.ViewModels
{
    public class TrophyPageViewModel : NotifierBase
    {
        private readonly TrophyDetailManager _trophyDetailManager = new TrophyDetailManager();
        private bool _isEmpty;
        private bool _isLoading;
        private ObservableCollection<TrophyDetailEntity.Trophy> _trophies;
        private TrophyDetailEntity _trophyDetailEntity;

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

        public async void SetTrophyList(string userName, string npCommunicationId)
        {
            IsLoading = true;
            TrophyDetailEntity trophys =
                await
                    _trophyDetailManager.GetTrophyDetailList(npCommunicationId,
                        userName, true,
                        App.UserAccountEntity);
            if (trophys == null)
            {
                IsLoading = false;
                return;
            }
            if (trophys.Trophies == null)
            {
                IsLoading = false;
                return;
            }
            foreach (TrophyDetailEntity.Trophy trophy in trophys.Trophies)
            {
                Trophies.Add(trophy);
            }
            if (!trophys.Trophies.Any())
            {
                IsEmpty = true;
            }
            IsLoading = false;
        }
    }
}