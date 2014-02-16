using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public enum MatchStatus
    {
        Submited,
        Confirmed,
        Invalid
    }

    public class Match
    {
        public long MatchID { get; set; }
        public int CreatorID { get; set; }
        public int GameID { get; set; }
        public int PlaygroundID { get; set; }
        public int CompetitionTypeID { get; set; }
        public long WinnerID { get; set; }
        public DateTime Date { get; set; }
        public MatchStatus Status { get; set; }

        public virtual User Creator { get; set; }
        public virtual Game Game { get; set; }
        public virtual Playground Playground { get; set; }
        public virtual CompetitionType CompetitionType { get; set; }
        public virtual List<CompetitorScore> Scores { get; set; }
        public virtual Competitor Winner { get; set; }
    }
}
