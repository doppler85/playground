using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Business.Contracts
{
    public class PagedResult<T> where T : class 
    {
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int TotalItems { get; set; }

        public List<T> Items { get; set; }
    }
}