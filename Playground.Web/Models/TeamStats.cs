using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class TeamStats : Team
    {
        public TeamStats(Team team)
        {
            CompetitorID = team.CompetitorID;
            CreationDate = team.CreationDate;
            Name = team.Name;
            PictureUrl = team.PictureUrl;
            Games = team.Games;
        }

        public GameCategory GameCategory { get; set;}
        public int TotalMatches { get; set; }
    }
}