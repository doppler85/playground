using System.ComponentModel.DataAnnotations;

namespace Playground.Model.Validation
{
    public static class CompetitionTypeValidator
    {
        private const int MIN_PLYERS_COUNT = 2;
        private const int MAX_PLAYERS_COUNT = 20;

        public static ValidationResult ValidateTeamMembers(CompetitionType competitionType)
        {
            ValidationResult retVal = ValidationResult.Success;

            if (competitionType != null && competitionType.CompetitorType == CompetitorType.Team)
            {
                if (competitionType.PlayersPerTeam < MIN_PLYERS_COUNT || competitionType.PlayersPerTeam > MAX_PLAYERS_COUNT)
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
