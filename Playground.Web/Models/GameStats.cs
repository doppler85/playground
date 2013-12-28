using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class GameStats : Game
    {
        public GameStats(Game game)
        {
            GameID = game.GameID;
            Title = game.Title;
            GameCategoryID = game.GameCategoryID;
            Category = game.Category;
            PictureUrl = game.PictureUrl;
        }

        public int TotalCompetitors { get; set; }
        public int TotalMatches { get; set; }
    }
}