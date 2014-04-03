using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class TrophyStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as TrophyDetailEntity.Trophy;
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            if (item == null) return resourceLoader.GetString("TrophyNotEarned/Text").Trim();
            if (item.ComparedUser != null)
            {
                return item.ComparedUser.Earned
                    ? resourceLoader.GetString("TrophyEarned/Text").Trim()
                    : resourceLoader.GetString("TrophyNotEarned/Text").Trim();
            }
            if (item.FromUser != null)
            {
                return item.FromUser.Earned
                    ? resourceLoader.GetString("TrophyEarned/Text").Trim()
                    : resourceLoader.GetString("TrophyNotEarned/Text").Trim();
            }
            return resourceLoader.GetString("TrophyNotEarned/Text").Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}