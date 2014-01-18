using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class GameCompetitionType
    {
        public int GameID { get; set; }
        public int CompetitionTypeID { get; set; }
        [NotMapped]
        public bool Selected { get; set; }


        public virtual Game Game { get; set; }
        public virtual CompetitionType CompetitionType { get; set; }
    }
}
