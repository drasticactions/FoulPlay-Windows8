using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace FoulPlay_Windows8.Tools
{
    public class TrophyTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new BitmapImage(new Uri("ms-appx:///Assets/phone_trophy_medium_hidden.png"));
            var trophyType = (string) value;
            switch (trophyType)
            {
                case "platinum":
                    return new BitmapImage(new Uri("ms-appx:///Assets/phone_trophy_medium_platinum.png"));
                case "gold":
                    return new BitmapImage(new Uri("ms-appx:///Assets/phone_trophy_medium_gold.png"));
                case "silver":
                    return new BitmapImage(new Uri("ms-appx:///Assets/phone_trophy_medium_silver.png"));
                case "bronze":
                    return new BitmapImage(new Uri("ms-appx:///Assets/phone_trophy_medium_bronze.png"));
                default:
                    return new BitmapImage(new Uri("ms-appx:///Assets/phone_trophy_medium_hidden.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}