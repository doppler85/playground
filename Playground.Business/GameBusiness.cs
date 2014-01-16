using Playground.Business.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public class GameBusiness : PlaygroundBusinessBase, IGameBusiness
    {
        public Result<Game> AddGameCategory(Game game)
        {
            throw new NotImplementedException();
        }

        public Result<Game> GetById(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
