using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model.Validation
{
    public static class PlayerValidator
    {
        public static ValidationResult ValidateGames(Player player)
        {
            ValidationResult retVal = ValidationResult.Success;

            if (player != null && player.Games != null && player.Games.Count < 1)
            {
                retVal = new ValidationResult("Player must have at least one game to compete in",
                    new string[] { "Player", "Games" }
                );
            }

            return retVal;
        }

    }
}
