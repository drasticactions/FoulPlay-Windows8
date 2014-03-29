using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using Foulplay_Windows8.Core.Entities;

namespace FoulPlay_Windows8.UserControls
{
    public sealed partial class RecentActivityUserControl : UserControl
    {
        private int StoryCount = 0;
        private RecentActivityEntity.Feed _feed;
        private bool _isLiked;

        private int _likeCount;


        public RecentActivityUserControl()
        {
            this.InitializeComponent();
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            StoryCount--;
            this.ParentPopup.DataContext = _feed.CondensedStories[StoryCount];
            if (StoryCount == 0)
            {
                BackButton.IsEnabled = false;
            }
            ForwardButton.IsEnabled = true;
            SetDataContentCondensedStories(_feed,
                _feed.CondensedStories[StoryCount]);
            ActivityPageCount.Text = string.Format("{0}/{1}", StoryCount + 1,
                _feed.CondensedStories.Count);
        }

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            StoryCount++;
            if (StoryCount >= _feed.CondensedStories.Count - 1)
            {
                ForwardButton.IsEnabled = false;
            }
            BackButton.IsEnabled = true;
            SetDataContentCondensedStories(_feed,
               _feed.CondensedStories[StoryCount]);
            this.ParentPopup.DataContext = _feed.CondensedStories[StoryCount];
            ActivityPageCount.Text = string.Format("{0}/{1}", StoryCount + 1,
                _feed.CondensedStories.Count);
        }

        public void OpenPopup()
        {
            this.ParentPopup.IsOpen = true;
        }

        public void ClosePopup()
        {
            this.ParentPopup.IsOpen = false;
        }

        public void SetOffset()
        {
            this.ParentPopup.HorizontalOffset = (Window.Current.Bounds.Width - 400) / 2;
            this.ParentPopup.VerticalOffset = (Window.Current.Bounds.Height - 500) / 2;
        }

        public void SetContext(RecentActivityEntity.Feed feed)
        {
            _feed = feed;
            _isLiked = feed.Liked;
            _likeCount = feed.LikeCount;
            this.ParentPopup.DataContext = feed;
            if (feed.CondensedStories != null)
            {
                ActivityPageCount.Text = string.Format("1/{0}", feed.CondensedStories.Count);
                SetDataContentCondensedStories(feed,
                        feed.CondensedStories[StoryCount]);
                ActivityPageGrid.Visibility = Visibility.Visible;
            }
            else
            {
                SetDataContent(feed);
            }
        }

        private void SetDataContent(RecentActivityEntity.Feed feed)
        {
            string storyType = feed.StoryType;
            RecentActivityEntity.Target target;
            switch (storyType)
            {
                case "STORE_PROMO":
                    MainImage.Source =
                       new BitmapImage(new Uri(feed.SmallImageUrl));
                     target = feed.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = feed.StoryComment;
                    break;
                case "SCREENSHOT_UPLOAD":
                    MainImage.Source =
                        new BitmapImage(new Uri(feed.SmallImageUrl));
                     target = feed.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br>", "\n").Replace("<b>", string.Empty).Replace("</b>", string.Empty).Trim();
                    break;
                case "TROPHY":
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_IMAGE_URL"));
                    if (target != null)
                        MainImage.Source = new BitmapImage(new Uri(target.Meta));
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_DETAIL"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta;
                    return;
                case "PLAYED_GAME":
                    MainImage.Source =
                       new BitmapImage(new Uri(feed.SmallImageUrl));
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("TITLE_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br>", "\n").Replace("<b>", string.Empty).Replace("</b>", string.Empty).Trim();
                    return;
                case "FRIENDED":
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("ONLINE_ID"));
                    if (target != null)
                    {
                        MainImage.Source = new BitmapImage(new Uri(target.ImageUrl));
                        ActivityHeaderTextBlock.Text = target.Meta;
                        ActivityTextBlock.Text = string.Empty;
                    }
                    return;
                case "BROADCASTING":
                    MainImage.Source =
                      new BitmapImage(new Uri(feed.SmallImageUrl));
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("TITLE_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br>", "\n").Replace("<b>", string.Empty).Replace("</b>", string.Empty).Trim();
                    return;
                case "PROFILE_PIC":
                    MainImage.Source =
                       new BitmapImage(new Uri(feed.LargeImageUrl));
                    ActivityHeaderTextBlock.Text = feed.Caption;
                    return;
                default:
                    return;
            }
        }

        private void SetDataContentCondensedStories(RecentActivityEntity.Feed feed, RecentActivityEntity.CondensedStory condensedStory)
        {
            string storyType = feed.StoryType;
            RecentActivityEntity.Target target;
            switch (storyType)
            {
                case "STORE_PROMO":
                    MainImage.Source =
                       new BitmapImage(new Uri(feed.SmallImageUrl));
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = feed.StoryComment;
                    break;
                case "SCREENSHOT_UPLOAD":
                    MainImage.Source =
                        new BitmapImage(new Uri(feed.SmallImageUrl));
                    target = feed.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br>", "\n").Replace("<b>", string.Empty).Replace("</b>", string.Empty).Trim();
                    break;
                case "TROPHY":
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_IMAGE_URL"));
                    if (target != null)
                        MainImage.Source = new BitmapImage(new Uri(target.Meta));
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TROPHY_DETAIL"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta;
                    return;
                case "PLAYED_GAME":
                    MainImage.Source =
                        new BitmapImage(new Uri(feed.SmallImageUrl));
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TITLE_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br><br>", "\n\n").Replace("<b>", string.Empty).Replace("</b>", string.Empty);
                    return;
                case "FRIENDED":
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("ONLINE_ID"));
                    if (target != null)
                    {
                        MainImage.Source = new BitmapImage(new Uri(target.ImageUrl));
                        ActivityHeaderTextBlock.Text = target.Meta;
                        ActivityTextBlock.Text = string.Empty;
                    }
                    return;
                case "BROADCASTING":
                    MainImage.Source =
                           new BitmapImage(new Uri(condensedStory.SmallImageUrl));
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("TITLE_NAME"));
                    if (target != null)
                        ActivityHeaderTextBlock.Text = target.Meta;
                    target = condensedStory.Targets.FirstOrDefault(o => o.Type.Equals("LONG_DESCRIPTION"));
                    if (target != null)
                        ActivityTextBlock.Text = target.Meta.Replace("<br>", "\n").Replace("<b>", string.Empty).Replace("</b>", string.Empty);
                    return;
                default:
                    return;
            }
        }
    }
}
