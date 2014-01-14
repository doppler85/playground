using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public class PlaygroundBusinessBase
    {
        protected IPlaygroundUow Uow { get; set; }

        protected int GetPage(int totalItems, int page, int count)
        {
            if (totalItems < page * count)
            {
                page = Math.Max((int)Math.Ceiling((decimal)totalItems / (decimal)count), 1);
            }

            return page;
        }
    }
}
