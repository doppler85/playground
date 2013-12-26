using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class PlayerStats : Player
    {
        public PlayerStats(Player player)
        {
            CompetitorID = player.CompetitorID;
            CreationDate = player.CreationDate;
            Name = player.Name;
            CompetitorPictureUrl = player.CompetitorPictureUrl;
            Games = player.Games;
            User = player.User;
        }

        public GameCategory GameCategory { get; set;}
        public int TotalMatches { get; set; }
    }
}