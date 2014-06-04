using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace FoulPlay_Windows8.Tools
{
    public class AvailablePlatformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            IEnumerable<string> stringEnumerable = (List<string>) value;
            return string.Join<string>(",", stringEnumerable);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}