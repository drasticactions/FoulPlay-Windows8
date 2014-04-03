using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FoulPlay_Windows8.Tools
{
    public class IsCurrentUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = (string) value;
            return App.UserAccountEntity.GetUserEntity().OnlineId.Equals(item)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}