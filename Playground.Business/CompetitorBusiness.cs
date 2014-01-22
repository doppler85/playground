using log4net;
using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public class CompetitorBusiness : PlaygroundBusinessBase, ICompetitorBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private IGameCategoryBusiness gameCategoryBusiness;
        

        public CompetitorBusiness(IPlaygroundUow uow, IGameCategoryBusiness gcBusiness)
        {
            this.Uow = uow;
            this.gameCategoryBusiness = gcBusiness;
        }

        public Result<PagedResult<Competitor>> GetCompetitors(int page, int count)
        {
            Result<PagedResult<Competitor>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Competitor> competitors = Uow.Competitors
                                                    .GetAll()
                                                    .OrderByDescending(c => c.CreationDate)
                                                    .Skip((page - 1) * count)
                                                    .Take(count)
                                                    .ToList();

                PagedResult<Competitor> result = new PagedResult<Competitor>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = competitors
                };

                retVal = ResultHandler<PagedResult<Competitor>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error getting list of competitors", ex);
                retVal = ResultHandler<PagedResult<Competitor>>.Erorr("Error getting list of competitors");
            }

            return retVal;
        }

        public void LoadCategories(List<Competitor> competitors)
        {
            try
            {
                foreach (Competitor competitor in competitors)
                {
                    competitor.Games = new List<GameCompetitor>();
                    GameCategory gameCategory = gameCategoryBusiness.GetByCompetitorId(competitor.CompetitorID);
                    competitor.Games.Add(new GameCompetitor
                    {
                        CompetitorID = competitor.CompetitorID,
                        Game = new Game()
                        {
                            Category = gameCategory,
                            GameCategoryID = gameCategory == null ? 0 : gameCategory.GameCategoryID
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error("Error loading categories for competitors list", ex);
            }
        }

        public void LoadCategories(List<Player> players)
        {
            try
            {
                foreach (Player player in players)
                {
                    player.Games = new List<GameCompetitor>();
                    GameCategory gameCategory = gameCategoryBusiness.GetByCompetitorId(player.CompetitorID);
                    player.Games.Add(new GameCompetitor
                    {
                        CompetitorID = player.CompetitorID,
                        Game = new Game()
                        {
                            Category = gameCategory,
                            GameCategoryID = gameCategory == null ? 0 : gameCategory.GameCategoryID
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error("Error loading categories for competitors list", ex);
            }
        }

        public void LoadCategories(List<Team> teams)
        {
            try
            {
                foreach (Team team in teams)
                {
                    team.Games = new List<GameCompetitor>();
                    GameCategory gameCategory = gameCategoryBusiness.GetByCompetitorId(team.CompetitorID);
                    team.Games.Add(new GameCompetitor
                    {
                        CompetitorID = team.CompetitorID,
                        Game = new Game()
                        {
                            Category = gameCategory,
                            GameCategoryID = gameCategory == null ? 0 : gameCategory.GameCategoryID
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error("Error loading categories for competitors list", ex);
            }
        }

        public void LoadUsers(List<Player> players)
        {
            try
            {
                foreach (Player player in players)
                {
                    player.User = Uow.Users.GetById(player.UserID);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error loading users for players list", ex);
            }
        }

        public Result<PagedResult<Player>> GetPlayersForUser(int page, int count, int userID)
        {
            Result<PagedResult<Player>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.UserID == userID)
                    .Count();

                page = GetPage(totalItems, page, count);

                List<Player> players = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
                                            .Where(p => p.UserID == userID)
                                            .OrderBy(p => p.Name)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                PagedResult<Player> result = new PagedResult<Player>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = players
                };

                retVal = ResultHandler<PagedResult<Player>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting list of players for user id: {0}", userID), ex);
                retVal = ResultHandler<PagedResult<Player>>.Erorr("Error getting list of players");
            }

            return retVal;
        }

        public Result<PagedResult<Team>> GetTeamsForUser(int page, int count, int userID)
        {
            Result<PagedResult<Team>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.UserID == userID)
                    .SelectMany(p => p.Teams)
                    .Select(t => t.Team)
                    .Count();

                page = GetPage(totalItems, page, count);

                List<Team> teams = Uow.Competitors
                                .GetAll()
                                .OfType<Player>()
                                .Where(p => p.UserID == userID)
                                .SelectMany(p => p.Teams)
                                .Select(t => t.Team)
                                .OrderBy(t => t.Name)
                                .Skip((page - 1) * count)
                                .Take(count)
                                .ToList();

                PagedResult<Team> result = new PagedResult<Team>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = teams
                };

                retVal = ResultHandler<PagedResult<Team>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting list of teams for user id: {0}", userID), ex);
                retVal = ResultHandler<PagedResult<Team>>.Erorr("Error getting list of teams");
            }

            return retVal;
        }

        public Result<PagedResult<Player>> GetPlayersForGame(int page, int count, int gameID)
        {
            Result<PagedResult<Player>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                .GetAll()
                                .OfType<Player>()
                                .Where(c => c.Games.Any(g => g.GameID == gameID))
                                .Count();

                page = GetPage(totalItems, page, count);

                List<Player> players = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
                                            .Where(c => c.Games.Any(g => g.GameID == gameID))
                                            .OrderByDescending(c => c.CreationDate)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                PagedResult<Player> result = new PagedResult<Player>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = players
                };

                retVal = ResultHandler<PagedResult<Player>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting list of players for game id: {0}", gameID), ex);
                retVal = ResultHandler<PagedResult<Player>>.Erorr("Error getting list of players");
            }

            return retVal;
        }

        public Result<PagedResult<Team>> GetTeamsForGame(int page, int count, int gameID)
        {
            Result<PagedResult<Team>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .OfType<Team>()
                                            .Where(c => c.Games.Any(g => g.GameID == gameID))
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Team> teams = Uow.Competitors
                                            .GetAll()
                                            .OfType<Team>()
                                            .Where(c => c.Games.Any(g => g.GameID == gameID))
                                            .OrderByDescending(c => c.CreationDate)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                PagedResult<Team> result = new PagedResult<Team>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = teams
                };

                retVal = ResultHandler<PagedResult<Team>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting list of teams for game id: {0}", gameID), ex);
                retVal = ResultHandler<PagedResult<Team>>.Erorr("Error getting list of teams");
            }

            return retVal;
        }

        public List<long> GetCompetitorIdsForUser(long userID)
        {
            List<long> retVal = null;
            try
            {
                List<long> playerIds = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.UserID == userID)
                    .Select(p => p.CompetitorID)
                    .ToList();

                List<long> teamIds = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.UserID == userID)
                    .SelectMany(p => p.Teams)
                    .Select(t => t.Team.CompetitorID)
                    .ToList();

                retVal = playerIds.Concat(teamIds).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Error retrieving competitor ids", ex);
            }
            return retVal;
        }

        public Result<Player> AddPlayer(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
