﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class SearchCompetitorArgs : SearchArgs
    {
        public int GameCategoryID { get; set; }
        public int CompetitorType { get; set; }
        public List<long> Ids { get; set; }
    }
}