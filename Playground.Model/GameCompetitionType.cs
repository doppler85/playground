using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class GameCompetitionType
    {
        public int GameID { get; set; }
        public int CompetitionTypeID { get; set; }

        public virtual Game Game { get; set; }
        public virtual CompetitionType CompetitionType { get; set; }
    }
}
