using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using FoulPlay.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class MaxPlayersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var session = (SessionInviteDetailEntity.Session)value;
            int members = session.Members.Count;
            return string.Format("{0}/{1}", members, session.SessionMaxUser);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
