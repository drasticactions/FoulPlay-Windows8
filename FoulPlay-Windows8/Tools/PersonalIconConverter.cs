using System;
using Windows.UI.Xaml.Data;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class PersonalIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as UserEntity;
            if (item == null) return string.Empty;
            if (item.personalDetail != null)
            {
                return !string.IsNullOrEmpty(item.personalDetail.ProfilePictureUrl)
                    ? item.personalDetail.ProfilePictureUrl
                    : item.AvatarUrl;
            }
            return item.AvatarUrl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}