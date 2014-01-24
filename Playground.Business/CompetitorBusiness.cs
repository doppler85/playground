using log4net;
using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public class CompetitorBusiness : PlaygroundBusinessBase, ICompetitorBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private IGameCategoryBusiness gameCategoryBusiness;
        
        public CompetitorBusiness(IPlaygroundUow uow, 
            IGameCategoryBusiness gcBusiness)
        {
            this.Uow = uow;
            this.gameCategoryBusiness = gcBusiness;
        }

        private bool CheckExistPerCategory(Player player, int gameCategoryID)
        {
            bool retVal = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .FirstOrDefault(p => p.Games.Any(g => g.Game.GameCategoryID == gameCategoryID) && 
                        p.UserID == player.UserID && 
                        p.CompetitorID != player.CompetitorID) != null;

            return retVal;
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

        public Result<PagedResult<Player>> GetPlayersForGameCategory(int page, int count, int gameCategoryID)
        {
            Result<PagedResult<Player>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
                                            .Where(c => c.Games.Any(g => g.Game.GameCategoryID == gameCategoryID))
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Player> players = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
                                            .Where(c => c.Games.Any(g => g.Game.GameCategoryID == gameCategoryID))
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
                log.Error(String.Format("Error getting list of players for game category. ID: {0}", gameCategoryID), ex);
                retVal = ResultHandler<PagedResult<Player>>.Erorr("Error getting list of players");
            }
            return retVal;
        }

        public Result<PagedResult<Team>> GetTeamsForGameCategory(int page, int count, int gameCategoryID)
        {
            Result<PagedResult<Team>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .OfType<Team>()
                                            .Where(c => c.Games.Any(g => g.Game.GameCategoryID == gameCategoryID))
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Team> teams = Uow.Competitors
                                            .GetAll(c => c.Games)
                                            .OfType<Team>()
                                            .Where(c => c.Games.Any(g => g.Game.GameCategoryID == gameCategoryID))
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
                log.Error(String.Format("Error getting list of teams for game category. ID: {0}", gameCategoryID), ex);
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
            Result<Player> retVal = null;
            try
            {
                Game game = Uow.Games.GetById(player.Games[0].GameID);
                if (CheckExistPerCategory(player, game.GameCategoryID))
                {
                    retVal = ResultHandler<Player>.Erorr("Only one player per game category allowed");
                }
                else
                {
                    player.CompetitorType = CompetitorType.Individual;
                    player.CreationDate = DateTime.Now;
                    Uow.Competitors.Add(player);
                    Uow.Commit();

                    retVal = ResultHandler<Player>.Sucess(player);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error adding player", ex);
                retVal = ResultHandler<Player>.Erorr("Erorr adding player");
            }

            return retVal;
        }

        public Result<Player> UpdatePlayer(Player player)
        {
            Result<Player> retVal = null;
            try
            {
                List<GameCompetitor> gameCompetitors = Uow.GameCompetitors
                    .GetAll()
                    .Where(gc => gc.CompetitorID == player.CompetitorID)
                    .ToList();
                foreach (GameCompetitor gc in gameCompetitors)
                {
                    Uow.GameCompetitors.Delete(gc);
                }
                foreach (GameCompetitor gc in player.Games.Where(g => g.Selected))
                {
                    gc.Competitor = null;
                    gc.Game = null;
                    Uow.GameCompetitors.Add(gc);
                }
                Uow.Competitors.Update(player, player.CompetitorID);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error("Error updating player", ex);
                retVal = ResultHandler<Player>.Erorr("Erorr updating player");
            }

            return retVal;
        }

        public void AssignImage(Competitor competitor, int userID, string fileSystemRoot, string urlRoot, string prefix, string extension)
        {
            try
            {
                // temp picture
                string sourceFilePath = String.Format("{0}{1}_{2}_{3}.{4}",
                                                                        fileSystemRoot,
                                                                        prefix,
                                                                        userID,
                                                                        0,
                                                                        extension);

                string destFilePath = String.Format("{0}{1}_{2}.{3}",
                                                                        fileSystemRoot,
                                                                        prefix,
                                                                        competitor.CompetitorID,
                                                                        extension);

                string destUrl = String.Format("{0}{1}_{2}.{3}",
                                                                        urlRoot,
                                                                        prefix,
                                                                        competitor.CompetitorID,
                                                                        extension);

                if (File.Exists(sourceFilePath))
                {
                    competitor.PictureUrl = destUrl;
                    File.Move(sourceFilePath, destFilePath);
                    File.Delete(sourceFilePath);
                }
                else
                {
                    competitor.PictureUrl = String.Empty;
                }

                Uow.Competitors.Update(competitor, competitor.CompetitorID);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error("Error assigning picture for competitor", ex);
            }
        }
    }
}
