using Playground.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Model.Util
{
    public class SearchAreaArgs : SearchArgs
    {
        public bool GlobalSearch { get; set; }
        public decimal StartLocationLatitude { get; set; }
        public decimal StartLocationLongitude { get; set; }
        public decimal EndLocationLatitude { get; set; }
        public decimal EndLocationLongitude { get; set; }
    }
}