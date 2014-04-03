using FoulPlay_Windows8.Common;
using FoulPlay_Windows8.Tools;

namespace FoulPlay_Windows8.ViewModels
{
    public class RecentActivityPageViewModel : NotifierBase
    {
        private RecentActivityScrollingCollection _recentActivityScrollingCollection;

        public RecentActivityScrollingCollection RecentActivityScrollingCollection
        {
            get { return _recentActivityScrollingCollection; }
            set
            {
                SetProperty(ref _recentActivityScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public void SetRecentActivityFeed()
        {
            RecentActivityScrollingCollection = new RecentActivityScrollingCollection
            {
                IsNews = false,
                StorePromo = false,
                UserAccountEntity = App.UserAccountEntity,
                Username = App.UserAccountEntity.GetUserEntity().OnlineId,
                PageCount = 0
            };
        }
    }
}