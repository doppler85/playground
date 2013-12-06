using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Playground.Model;
using System.Web.Http;
using System.Net.Http;

namespace Playground.Web.Controllers
{
    [Authorize]
    public class UserController : ApiBaseController
    {
        public UserController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        // api/user/getplayers
        [HttpGet]
        [ActionName("getplayers")]
        public List<Player> GetPlayers()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage AddPlayer()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage UpdatePlayer()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage DeletePlayer()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("getteams")]
        public List<Team> GetTeams()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage AddTeam()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage UpdateTeam()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage DeleteTeam()
        {
            throw new NotImplementedException();
        }
    }
}