using System.Collections.ObjectModel;
using System.Linq;
using FoulPlay.Core.Entities;
using FoulPlay.Core.Managers;
using FoulPlay_Windows8.Common;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;

namespace FoulPlay_Windows8.ViewModels
{
    public class SessionInvitePopupViewModel : NotifierBase
    {
        private SessionInviteEntity.Invitation _sessionInvitation;
        private SessionInviteDetailEntity _sessionInviteDetailEntity;

        private ObservableCollection<SessionInviteMember> _sessionInviteMemberCollection =
            new ObservableCollection<SessionInviteMember>();

        private UserEntity _user;

        public SessionInviteEntity.Invitation SessionInvitation
        {
            get { return _sessionInvitation; }
            set
            {
                SetProperty(ref _sessionInvitation, value);
                OnPropertyChanged();
            }
        }

        public SessionInviteDetailEntity SessionInviteDetailEntity
        {
            get { return _sessionInviteDetailEntity; }
            set
            {
                SetProperty(ref _sessionInviteDetailEntity, value);
                OnPropertyChanged();
            }
        }

        public UserEntity User
        {
            get { return _user; }
            set
            {
                SetProperty(ref _user, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SessionInviteMember> SessionInviteMembers
        {
            get { return _sessionInviteMemberCollection; }
            set
            {
                SetProperty(ref _sessionInviteMemberCollection, value);
                OnPropertyChanged();
            }
        }

        public void SetInvite(SessionInviteEntity.Invitation invite)
        {
            SessionInvitation = invite;
        }

        public async void GetUser(string userName)
        {
            User = await UserManager.GetUser(userName, App.UserAccountEntity);
        }

        public async void GetSessionInvite(string inviteId)
        {
            var sessionInviteManager = new SessionInviteManager();
            SessionInviteDetailEntity = await sessionInviteManager.GetInviteInformation(inviteId, App.UserAccountEntity);
            if (SessionInviteDetailEntity == null) return;
            if (SessionInviteDetailEntity.session == null) return;
            if (SessionInviteDetailEntity.session.Members == null) return;
            foreach (
                SessionInviteMember newMessage in
                    SessionInviteDetailEntity.session.Members.Select(member => new SessionInviteMember {Member = member})
                )
            {
                GetAvatar(newMessage, App.UserAccountEntity);
                SessionInviteMembers.Add(newMessage);
            }
        }

        private async void GetAvatar(SessionInviteMember member, UserAccountEntity userAccountEntity)
        {
            UserEntity user = await UserManager.GetUserAvatar(member.Member.OnlineId, userAccountEntity);
            member.AvatarUrl = user.AvatarUrl;
            OnPropertyChanged("SessionInviteMembers");
        }

        public class SessionInviteMember : NotifierBase
        {
            private string _avatarUrl;

            public string AvatarUrl
            {
                get { return _avatarUrl; }
                set
                {
                    SetProperty(ref _avatarUrl, value);
                    OnPropertyChanged();
                }
            }

            public SessionInviteDetailEntity.Member Member { get; set; }
        }
    }
}