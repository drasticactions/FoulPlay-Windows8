using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using FoulPlay.Core.Entities;
using FoulPlay_Windows8.ViewModels;
using FoulPlay_Windows8.Views;

namespace FoulPlay_Windows8.UserControls
{
    public sealed partial class SessionInviteUserControl : UserControl
    {
        private SessionInvitePopupViewModel _vm;
        public SessionInviteUserControl()
        {
            this.InitializeComponent();
        }

        public void OpenPopup()
        {
            ParentPopup.IsOpen = true;
        }

        public void ClosePopup()
        {
            ParentPopup.IsOpen = false;
        }

        public void SetOffset()
        {
            ParentPopup.HorizontalOffset = (Window.Current.Bounds.Width - 400) / 2;
            ParentPopup.VerticalOffset = (Window.Current.Bounds.Height - 500) / 2;
        }

        public void SetContext(SessionInviteEntity.Invitation invite)
        {
            if (invite == null) return;
            if (invite.FromUser == null) return;
            _vm = (SessionInvitePopupViewModel)DataContext;
            _vm.SetInvite(invite);
            _vm.GetUser(invite.FromUser.OnlineId);
            _vm.GetSessionInvite(invite.InvitationId);
        }

        private void SendMessageToUserButton_OnClick(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            if (frame == null) return;
            if (_vm.User == null) return;
            ClosePopup();
            frame.Navigate(typeof(FriendPage), _vm.User.OnlineId);
        }
    }
}
