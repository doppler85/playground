using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class UserStats : User
    {
        public UserStats(User user)
        {
            UserID = user.UserID;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Gender = user.Gender;
            PictureUrl = user.PictureUrl;
        }

        public int TotalGames { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalTeams { get; set; }
        public int TotalMatches { get; set; }
    }
}