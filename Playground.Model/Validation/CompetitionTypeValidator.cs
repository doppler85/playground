using Playground.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Playground.Model.Validation
{
    public static class CompetitionTypeValidator
    {
        public static ValidationResult ValidateTeamMembers(CompetitionType competitionType)
        {
            ValidationResult retVal = ValidationResult.Success;

            if (competitionType != null && competitionType.CompetitorType == CompetitorType.Team)
            {
                if (competitionType.PlayersPerTeam < Constants.Validation.MinPlyersCountPerTeam || competitionType.PlayersPerTeam > Constants.Validation.MaxPlayersCountPerTeam)
                {
                    retVal = new ValidationResult(String.Format("For team games players per team myst be between {0} and {1}", 
                        Constants.Validation.MinPlyersCountPerTeam,
                        Constants.Validation.MaxPlayersCountPerTeam),
                        new string[] { "CompetitorType", "PlayersPerTeam" }
                    );
                }
            }

            return retVal;
        }
    }
}
