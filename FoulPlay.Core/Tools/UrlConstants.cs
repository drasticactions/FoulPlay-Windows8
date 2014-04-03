﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foulplay_Windows8.Core.Tools
{
    public class UrlConstants
    {
        public static readonly string VerifyUser = "https://vl.api.np.km.playstation.net/vl/api/v1/mobile/users/me/info";

        public const string UstreamBaseUrl = "https://ps4api.ustream.tv/media.json?";

        public const string TwitchBaseUrl = "https://api.twitch.tv/api/orbis/streams?";
    }

    public class UstreamUrlConstants
    {
        public const string FilterBase = "filter[{0}]";

        public const string Platform = "platform";

        public const string Type = "type";

        public const string PlatformPs4 = "PS4";

        public const string Interactive = "interactive";

        public const string Sort = "sort";
    }
}