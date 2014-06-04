using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class FriendRequestVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var friend = value as UserEntity;
            if (friend == null) return Visibility.Collapsed;
            ;
            if (string.IsNullOrEmpty(friend.Relation))
                return Visibility.Collapsed;
            if (friend.Relation.Equals("friend of friends") || friend.Relation.Equals("no relationship"))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}