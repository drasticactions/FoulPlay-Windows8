using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace FoulPlay_Windows8.Tools
{
    public class TrophyRareTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var trophyValue = (int)value;
            var resourceLoader = ResourceLoader.GetForCurrentView(); 
            switch (trophyValue)
            {
                case 0:
                    return resourceLoader.GetString("TrophyUltraRare/Text").Trim();
                case 1:
                    return resourceLoader.GetString("TrophyVeryRare/Text").Trim();
                case 2:
                    return resourceLoader.GetString("TrophyRare/Text").Trim();
                case 3:
                    return resourceLoader.GetString("TrophyCommon/Text").Trim();
                case 4:
                    return resourceLoader.GetString("TrophyCommon/Text").Trim();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
