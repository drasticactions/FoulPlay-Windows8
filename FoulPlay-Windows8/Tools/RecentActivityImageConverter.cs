using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class RecentActivityImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var item = (RecentActivityEntity.Feed)value;
            switch (item.StoryType)
            {
                case "FRIENDED":
                    var target = item.Targets.FirstOrDefault(o => o.Type.Equals("ONLINE_ID"));
                    return target != null ? target.ImageUrl : null;
                default:
                    return item.ThumbnailImageUrl;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
