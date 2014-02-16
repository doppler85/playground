using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class MatchNotification : Notification
    {
        public long MatchID { get; set; }

        public virtual Match Match { get; set; }
    }
}
