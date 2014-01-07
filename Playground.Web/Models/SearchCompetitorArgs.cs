using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class SearchCompetitorArgs
    {
        public int Page { get; set; }
        public int Count { get; set; }
        public int GameCategoryID { get; set; }
        public int CompetitorType { get; set; }
        public string Search { get; set; }
        public List<long> Ids { get; set; }
    }
}