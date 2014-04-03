using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FoulPlay_Windows8.Tools
{
    public class InviteImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            const string url = "/Assets/appbar.hardware.headset.dark.png";
            return string.IsNullOrEmpty((string)value) ? new Uri(url, UriKind.RelativeOrAbsolute) : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
