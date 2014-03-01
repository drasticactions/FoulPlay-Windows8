using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Foulplay_Windows8.Core.Entities
{
    public class RecentActivityEntity
    {
        public class Target
        {
            public string Meta { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
        }

        public class CondensedStory
        {
            public string Caption { get; set; }
            public List<Target> Targets { get; set; }
            public string CaptionTemplate { get; set; }
            public List<CaptionComponent> CaptionComponents { get; set; }
            public string StoryId { get; set; }
            public string StoryType { get; set; }
            public Source Source { get; set; }
            public string SmallImageUrl { get; set; }
            public string SmallImageAspectRatio { get; set; }
            public string LargeImageUrl { get; set; }
            public string ThumbnailImageUrl { get; set; }
            public DateTime Date { get; set; }
            public double Relevancy { get; set; }
            public int CommentCount { get; set; }
            public int LikeCount { get; set; }
            public string TitleId { get; set; }
            public string ProductId { get; set; }
            public string ProductUrl { get; set; }
            public bool Liked { get; set; }
            public string ServiceProviderName { get; set; }
            public bool Reshareable { get; set; }
        }

        public class CaptionComponent
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Source
        {
            public string Meta { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
        }

        public class Feed
        {
            public string Caption { get; set; }

            public List<CondensedStory> CondensedStories { get; set; }

            public List<Target> Targets { get; set; }
            public string CaptionTemplate { get; set; }
            public List<CaptionComponent> CaptionComponents { get; set; }
            public string StoryId { get; set; }
            public string StoryType { get; set; }
            public Source Source { get; set; }
            public string SmallImageUrl { get; set; }
            public string SmallImageAspectRatio { get; set; }
            public string LargeImageUrl { get; set; }
            public string ThumbnailImageUrl { get; set; }
            public DateTime Date { get; set; }
            public double Relevancy { get; set; }
            public int CommentCount { get; set; }
            public int LikeCount { get; set; }
            public string TitleId { get; set; }
            public string ProductId { get; set; }
            public string ProductUrl { get; set; }
            public bool Liked { get; set; }
            public string ServiceProviderName { get; set; }
            public bool Reshareable { get; set; }
        }
        public List<Feed> feed { get; set; }
    }
}
