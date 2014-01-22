using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface ICompetitorBusiness
    {
        Result<PagedResult<Competitor>> GetCompetitors(int page, int count);

        Result<PagedResult<Player>> GetPlayersForUser(int page, int count, int userID);

        Result<PagedResult<Team>> GetTeamsForUser(int page, int count, int userID);

        Result<PagedResult<Player>> GetPlayersForGame(int page, int count, int gameID);

        Result<PagedResult<Team>> GetTeamsForGame(int page, int count, int gameID);

        void LoadCategories(List<Competitor> competitors);

        void LoadCategories(List<Player> players);

        void LoadCategories(List<Team> teams);

        void LoadUsers(List<Player> players);

        List<long> GetCompetitorIdsForUser(long userID);
    }
}
