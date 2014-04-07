using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.Tools
{
    public class ActivityTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StoreActivityDataTemplate { get; set; }

        public DataTemplate ActivityDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item,
            DependencyObject container)
        {
            var feedItem = item as RecentActivityEntity.Feed;
            if (feedItem == null) return null;
            var uiElement = container as UIElement;
            switch (feedItem.StoryType)
            {
                case "STORE_PROMO":
                    #if !WINDOWS_PHONE_APP
                    VariableSizedWrapGrid.SetRowSpan(uiElement, 2);
                    VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                    #endif
                    return StoreActivityDataTemplate;
                default:
                    #if !WINDOWS_PHONE_APP
                    VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                    VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                    #endif
                    return ActivityDataTemplate;
            }
        }
    }
}