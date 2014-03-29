﻿using System;
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
    public class MessagePageViewModel : NotifierBase
    {
        public MessagePageViewModel() 
        {
            _messageEntity = new MessageEntity();
            _messageGroupCollection = new ObservableCollection<MessageGroupItem>();
        }
        private MessageEntity _messageEntity;
        private ObservableCollection<MessageGroupItem> _messageGroupCollection = new ObservableCollection<MessageGroupItem>();
        /// <summary>
        /// TODO: Seperate to new class, use ISupportIncrementalLoading
        /// </summary>
        public class MessageGroupItem : NotifierBase
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

            public MessageEntity.Message Message { get; set; }

            public MessageEntity.MessageGroup MessageGroup { get; set; }
        }


        public ObservableCollection<MessageGroupItem> MessageGroupCollection
        {
            get { return _messageGroupCollection; }
            set
            {
                SetProperty(ref _messageGroupCollection, value);
                OnPropertyChanged();
            }
        }

        public async void SetMessages(string messageGroupId, UserAccountEntity userAccountEntity)
        {
            var messageManager = new MessageManager();
            _messageEntity = await messageManager.GetGroupConversation(messageGroupId, App.UserAccountEntity);
            if (_messageEntity == null)
                return;
            foreach (var newMessage in _messageEntity.messages.Select(message => new MessageGroupItem { Message = message }))
            {
                GetAvatar(newMessage, userAccountEntity);
                MessageGroupCollection.Add(newMessage);
            }
        }

        private async void GetAvatar(MessageGroupItem message, UserAccountEntity userAccountEntity)
        {
            var user = await UserManager.GetUserAvatar(message.Message.senderOnlineId, userAccountEntity);
            message.AvatarUrl = user.AvatarUrl;
            OnPropertyChanged("MessageGroupCollection");
        }
    }
}