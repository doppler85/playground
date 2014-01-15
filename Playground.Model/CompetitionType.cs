using Playground.Model.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    [CustomValidation(typeof(CompetitionTypeValidator), "ValidateTeamMembers")]
    public class CompetitionType
    {
        public int CompetitionTypeID { get; set; }
        [Required]
        public CompetitorType CompetitorType { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Range(1, 20)]
        public int CompetitorsCount { get; set; }
        [DefaultValue(0)]
        public int PlayersPerTeam { get; set; }

        public virtual List<GameCompetitionType> Games { get; set; }
    }
}
