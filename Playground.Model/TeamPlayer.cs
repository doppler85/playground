using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class TeamPlayer
    {
        public long TeamID { get; set; }
        public long PlayerID { get; set; }

        public Team Team { get; set; }
        public Player Player { get; set; }
    }
}
