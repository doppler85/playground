using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public enum CompetitorType
    {
        Individual, 
        Team
    }

    public class CompetitionType
    {
        public int CompetitionTypeID { get; set; }
        [Required]
        public CompetitorType CompetitorType { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, 20)]
        public int CompetitorsCount { get; set; }

        public virtual List<GameCompetitionType> Games { get; set; }
    }
}
