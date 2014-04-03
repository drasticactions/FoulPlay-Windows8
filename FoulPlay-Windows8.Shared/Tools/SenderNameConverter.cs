using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace FoulPlay_Windows8.Tools
{
    public class SenderNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            string fromUser = resourceLoader.GetString("FromUser.Text");
            return value == null ? null : string.Format("{0}: {1}", fromUser, (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
