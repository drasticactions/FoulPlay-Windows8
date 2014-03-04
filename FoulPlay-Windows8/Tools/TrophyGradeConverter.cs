using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace FoulPlay_Windows8.Tools
{
    public class TrophyGradeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var trophyType = (string)value;
            var resourceLoader = ResourceLoader.GetForCurrentView(); 
            switch (trophyType)
            {
                case "platinum":
                    return resourceLoader.GetString("TrophyPlatinum/Text").Trim();
                case "gold":
                    return resourceLoader.GetString("TrophyGold/Text").Trim();
                case "silver":
                    return resourceLoader.GetString("TrophySilver/Text").Trim();
                case "bronze":
                    return resourceLoader.GetString("TrophyBronze/Text").Trim();
                default:
                    return resourceLoader.GetString("TrophyHidden/Text").Trim();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
