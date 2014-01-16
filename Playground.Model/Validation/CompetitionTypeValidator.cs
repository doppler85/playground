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
                if (competitionType.PlayersPerTeam < 2 || competitionType.PlayersPerTeam > 20)
                {
                    retVal = new ValidationResult("For team games players per team myst be between 2 and 20",
                        new string[] { "CompetitorType", "PlayersPerTeam" }
                    );
                }
            }

            return retVal;
        }
    }
}
