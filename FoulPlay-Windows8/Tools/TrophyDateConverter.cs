using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class TrophyDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as TrophyDetailEntity.Trophy;
            if (item == null) return Visibility.Collapsed;
            if (item.ComparedUser != null)
            {
                return item.ComparedUser.EarnedDate != null
                    ? DateTime.Parse(item.ComparedUser.EarnedDate).ToLocalTime().ToString()
                    : string.Empty;
            }
            if (item.FromUser != null)
            {
                return item.FromUser.EarnedDate != null
                    ? DateTime.Parse(item.FromUser.EarnedDate).ToLocalTime().ToString()
                    : string.Empty;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}