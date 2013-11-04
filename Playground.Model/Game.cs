using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class Game
    {
        public int GameID { get; set; }
        public int GameCategoryID { get; set; }
        public string Title { get; set; }

        public virtual GameCategory Category { get; set; }
        public virtual List<Competitor> Competitors { get; set; }
        public virtual List<CompetitionType> CompetitionTypes { get; set; }
    }
}
