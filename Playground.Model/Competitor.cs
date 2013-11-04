using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public enum CompetitorStatus
    {
        Active,
        Inactive
    }
    
    public abstract class Competitor
    {
        public long CompetitorID { get; set; }
        public int GameID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string PictureSource { get; set; }
        public CompetitorStatus Status { get; set; }

        public virtual Game Game { get; set; }
        public virtual List<CompetitorScore> Scores { get; set; }
    }
}
