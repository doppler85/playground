using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface IGameCategoryBusiness
    {
        Result<GameCategory> GetById(int id);

        int TotalGamesCount(int gameCategoryID);

        int TotalCompetitorsCount(int gameCategoryID);

        int TotalMatchesCount(int gameCategoryID);
        
        GameCategory GetByCompetitorId(long competitorID);

        Result<PagedResult<GameCategory>> GetGameCategories(int page, int count);

        Result<List<GameCategory>> GetGameCategoriesByCompetitorType(CompetitorType competitorType);

        Result<GameCategory> AddGameCategory(GameCategory gameCategory);

        Result<GameCategory> UpdateGameCategory(GameCategory gameCategory);
        
        bool DeleteGameCategory(int id);
    }
}
