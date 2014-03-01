using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class ActivityTemplateSelector : DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item,
            Windows.UI.Xaml.DependencyObject container)
        {
            var feedItem = item as RecentActivityEntity.Feed;

            var uiElement = container as UIElement;

            VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
            VariableSizedWrapGrid.SetRowSpan(uiElement, 1);

            return App.Current.Resources["ProjectItemTemplate"] as DataTemplate;

        }
    }
}
