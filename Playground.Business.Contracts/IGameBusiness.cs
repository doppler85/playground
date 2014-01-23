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

        Result<PagedResult<Game>> FilterByCategory(int page, int count, int gameCategoryID);

        Result<List<Game>> FilterByCategoryAndCompetitionType(int gameCategoryID, CompetitorType competitorType);

        bool DeleteGame(int gameID);

        void LoadImages(List<Game> games);
    }
}
