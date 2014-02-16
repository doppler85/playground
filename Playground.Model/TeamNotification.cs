using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class TeamNotification : Notification
    {
        public long TeamID { get; set; }

        public virtual Team Team { get; set; } 
    }
}
