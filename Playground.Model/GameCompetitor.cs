using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class GameCompetitor
    {
        public int GameID { get; set; }
        public long CompetitorID { get; set; }

        public Game Game { get; set; }
        public Competitor Competitor { get; set; }
    }
}
