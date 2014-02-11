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
        Result<Player> GetPlayerById(long playerID);

        Result<Team> GetTeamById(long teamID);

        Result<PagedResult<Competitor>> GetCompetitors(int page, int count);

        Result<PagedResult<Player>> GetPlayers(int page, int count);

        Result<PagedResult<Team>> GetTeams(int page, int count);

        Result<List<Competitor>> FilterByUserAndCategory(int userID, int gameCategoryID);

        Result<PagedResult<Competitor>> GetTopCompetitorsByDate(int page, int count, DateTime startDate);

        Result<PagedResult<Competitor>> SearchAndExcludeByCategoryAndCompetitorType(int page, 
            int count, 
            List<long> excludeIds, 
            int gameCategoryID,
            CompetitorType competitorType,
            string search);

        Result<PagedResult<Player>> GetPlayersForUser(int page, int count, int userID);

        Result<PagedResult<Team>> GetTeamsForUser(int page, int count, int userID);

        Result<PagedResult<Player>> GetPlayersForGame(int page, int count, int gameID);

        Result<PagedResult<Team>> GetTeamsForGame(int page, int count, int gameID);

        Result<PagedResult<Player>> FilterPlayersByGameCategory(int page, int count, int gameCategoryID);

        Result<PagedResult<Player>> SearchPlayersForGameCategory(int page, int count, int userID, int gameCategoryID, List<long> ids, string search);

        Result<PagedResult<Team>> FilterTeamsByGameCategory(int page, int count, int gameCategoryID);

        Result<PagedResult<Player>> FilterPlayersByTeam(int page, int count, long teamID);

        Result<PagedResult<Team>> FilterTeamsByPlayer(int page, int count, long playerID);

        Result<Player> GetPlayerForGameCategory(int userID, int gameCategoryID);

        void LoadCategories(List<Competitor> competitors);

        void LoadCategories(List<Player> players);

        void LoadCategories(List<Team> teams);

        void LoadUsers(List<Player> players);

        List<long> GetCompetitorIdsForUser(long userID);

        List<long> GetPlayerIdsForTeam(long teamID);

        Result<Player> AddPlayer(Player player);

        Result<Player> UpdatePlayer(Player player);

        Result<Player> GetUpdatePlayer(long playerID);

        Result<Team> AddTeam(Team team);

        Result<Team> UpdateTeam(Team team);

        Result<Team> GetUpdateTeam(long teamID, int userID);

        bool DeleteCompetitor(long competitorID);

        void AssignImage(Competitor competitor, int userID, string fileSystemRoot, string urlRoot, string prefix, string extension);

        bool CheckUserCompetitor(int userID, long competitorID);

        Result<bool> ConfirmScore(CompetitorScore competitorScore);

        int TotalMatchesCount(long competitorID);
    }
}
