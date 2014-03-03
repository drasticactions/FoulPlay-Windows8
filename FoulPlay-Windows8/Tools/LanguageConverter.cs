using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace FoulPlay_Windows8.Tools
{
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var languagesUsed = (List<string>) value;
            var languageList = languagesUsed.Select(ParseLanguageVariable).ToList();
            //MyLanguagesBlock.Text = string.Join("," + Environment.NewLine, languageList);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
