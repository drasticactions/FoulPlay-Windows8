using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace FoulPlay_Windows8.Tools
{
    public class OnlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var colorValue = (string)value;
            if (colorValue.Equals("online"))
            {
                return new SolidColorBrush(Colors.Blue);
            }
            return colorValue.Equals("offline") ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Yellow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
