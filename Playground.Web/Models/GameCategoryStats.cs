using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class GameCategoryStats : GameCategory
    {
        public GameCategoryStats(GameCategory category)
        {
            GameCategoryID = category.GameCategoryID;
            Title = category.Title;
            GameCategoryPictureUrl = category.GameCategoryPictureUrl;
        }

        public int TotalGames { get; set; }
        public int TotalCompetitors { get; set; }
        public int TotalMatches { get; set; }
    }
}