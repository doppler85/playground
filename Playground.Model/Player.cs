using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class Player: Competitor
    {
        public int UserID { get; set; }

        public virtual User User { get; set; }
        public virtual List<TeamPlayer> Teams { get; set; }
    }
}
