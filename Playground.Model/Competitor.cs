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
        public CompetitorType CompetitorType { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string PictureSource { get; set; }
        public CompetitorStatus Status { get; set; }
        [NotMapped]
        public bool IsCurrentUserCompetitor { get; set; }
        [NotMapped]
        public string CompetitorPictureUrl { get; set; }

        public virtual List<GameCompetitor> Games { get; set; }
        public virtual List<CompetitorScore> Scores { get; set; }
    }
}
