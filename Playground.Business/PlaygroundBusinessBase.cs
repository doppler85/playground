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
    }
}
