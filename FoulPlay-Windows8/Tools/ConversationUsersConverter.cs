using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class ConversationUsersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var message = (MessageGroupEntity.MessageGroup)value;
            var messageGroupDetail = message.MessageGroupDetail;
            var user = App.UserAccountEntity.GetUserEntity();
            var stringEnumerable = messageGroupDetail.Members.Where(member => !member.OnlineId.Equals(user.OnlineId)).Select(member => member.OnlineId).ToList();
            return string.Join<string>(",", stringEnumerable);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
