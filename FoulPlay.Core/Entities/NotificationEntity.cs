using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foulplay_Windows8.Core.Entities
{
    public class NotificationEntity
    {
        public class Notification
        {
            public string NotificationGroup { get; set; }
            public object NotificationId { get; set; }
            public bool SeenFlag { get; set; }
            public string ReceivedDate { get; set; }
            public string UpdateDate { get; set; }
            public string ActionUrl { get; set; }
            public string Message { get; set; }
        }

        public List<Notification> Notifications { get; set; }
        public int Start { get; set; }
        public int Size { get; set; }
        public int TotalResults { get; set; }
    }
}
