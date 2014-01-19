using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class SearchArgs
    {
        public int Page { get; set; }
        public int Count { get; set; }
        public string Search { get; set; }
    }
}