using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface IGameBusiness
    {
        Result<Game> GetById(Game game);
        Result<Game> AddGameCategory(Game game);
    }
}
