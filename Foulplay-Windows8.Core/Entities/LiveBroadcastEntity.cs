using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foulplay_Windows8.Core.Entities
{
    public class LiveBroadcastEntity
    {
        public string Title { get; set; }

        public bool FromTwitch { get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public string GameTitle { get; set; }

        public string GameMetadata { get; set; }

        public bool IsOnline { get; set; }

        public string OnlineTime { get; set; }

        public string BroadcastId { get; set; }

        public string Platform { get; set; }

        public string Language { get; set; }

        public string Viewers { get; set; }

        public string SocialStream { get; set; }

        public string PreviewThumbnail { get; set; }

        public string Service { get; set; }

        public void ParseFromTwitch(TwitchEntity.Stream twitchStream)
        {
            try
            {
                Title = twitchStream.status;
                FromTwitch = true;
                Description = twitchStream.name;
                UserName = twitchStream.sce_user_online_id;
                GameTitle = twitchStream.sce_title_name;
                OnlineTime = DateTime.Parse(twitchStream.stream_up).ToLocalTime().ToString();
                BroadcastId = twitchStream.broadcast_id as string;
                Platform = twitchStream.sce_platform;
                Language = twitchStream.sce_title_language;
                GameMetadata = twitchStream.sce_title_metadata;
                PreviewThumbnail = twitchStream.preview;
                Viewers = twitchStream.viewers.ToString();
                Service = "Twitch";
            }
            catch (Exception)
            {
                return;
            }
        }

        public void ParseFromUstream(UstreamEntity.Item ustreamEntity)
        {
            try
            {
                Service = "UStream";
                Title = ustreamEntity.media.title;
                PreviewThumbnail = ustreamEntity.media.thumbnail.live;
                Description = ustreamEntity.media.description;
                GameTitle = ustreamEntity.media.description;
                Viewers = ustreamEntity.media.stats.viewer.ToString();
                SocialStream = ustreamEntity.media.stats.socialstream.ToString();
                var testDate = new DateTime().AddSeconds(ustreamEntity.media.stream_started_at);
                OnlineTime = testDate.ToLocalTime().ToString();
            }
            catch (Exception)
            {
                return;
            }
        }
        
    }
}
