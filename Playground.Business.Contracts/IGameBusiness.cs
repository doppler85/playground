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
        Result<Game> GetById(int gameID);

        int TotalCompetitorsCount(int gameID);

        int TotalMatchesCount(int gameID);

        Result<PagedResult<Game>> FilterByCategory(int page, int count, int gameCategoryID);

        Result<List<Game>> FilterByCategoryAndCompetitionType(int gameCategoryID, CompetitorType competitorType);

        Result<PagedResult<Game>> FilterByPlayground(int page, int count, int playgroundID);

        Result<PagedResult<Game>> SearchAvailableByPlayground(int page, int count, int playgroundID, string search);

        Result<Game> AddGame(Game game);

        Result<Game> UpdateGame(Game game);

        bool DeleteGame(int gameID);

        void LoadImages(List<Game> games);
    }
}
