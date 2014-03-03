using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace FoulPlay_Windows8.Tools
{
    public class TrophyTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new BitmapImage(new Uri("ms-appx:///Assets/Trophy-icon-Hidden.png"));
            var trophyType = (string)value;
            switch (trophyType)
            {
                case "platinum":
                    return new BitmapImage(new Uri("ms-appx:///Assets/Trophy-icon-plat.png"));
                case "gold":
                    return new BitmapImage(new Uri("ms-appx:///Assets/Trophy-icon-Gold.png"));
                case "silver":
                    return new BitmapImage(new Uri("ms-appx:///Assets/Trophy-icon-Silver.png"));
                case "bronze":
                    return new BitmapImage(new Uri("ms-appx:///Assets/Trophy-icon-Bronze.png"));
                default:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Trophy-icon-Hidden.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
