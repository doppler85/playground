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
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        public string PictureUrl { get; set; }

        public virtual GameCategory Category { get; set; }
        public virtual List<GameCompetitor> Competitors { get; set; }
        public virtual List<GameCompetitionType> CompetitionTypes { get; set; }
        public virtual List<Match> Matches { get; set; }
    }
}
