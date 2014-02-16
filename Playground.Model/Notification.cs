using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public enum NotificationType 
    {
        TeamCreated,
        MatchAdded
    }

    public abstract class Notification
    {
        public long NotificationID { get; set; }
        public NotificationType NotificationType { get; set; } 
        public int UserID { get; set; }

        public virtual User User { get; set; }
    }
}
