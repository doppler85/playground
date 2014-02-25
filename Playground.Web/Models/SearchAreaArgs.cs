using Playground.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Models
{
    public class SearchAreaArgs
    {
        public decimal StartLocationLatitude { get; set; }
        public decimal StartLocationLongitude { get; set; }
        public decimal EndLocationLatitude { get; set; }
        public decimal EndLocationLongitude { get; set; }
        public int MaxResults { get; set; }
    }
}