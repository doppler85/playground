using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Playground.Web.Controllers
{  
    public class GameController : ApiBaseController
    {
        public GameController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        [HttpGet]
        public List<GameCategory> Categories()
        {
            List<GameCategory> games = Uow.GameCategories.GetAll("Games").ToList();
            return games;
        }
    }
}
