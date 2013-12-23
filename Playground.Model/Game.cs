using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public string GamePictureUrl { get; set; }

        public virtual GameCategory Category { get; set; }
        public virtual List<GameCompetitor> Competitors { get; set; }
        public virtual List<GameCompetitionType> CompetitionTypes { get; set; }
    }
}
