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

        public Result<Player> GetPlayerById(long playerID)
        {
            Result<Player> retVal = null;
            try
            {
                Player player = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .FirstOrDefault(p => p.CompetitorID == playerID);

                retVal = ResultHandler<Player>.Sucess(player);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving player, ID: {0}", playerID), ex);
                retVal = ResultHandler<Player>.Erorr("Error retreiving player");

            }

            return retVal;
        }

        public Result<Team> GetTeamById(long teamID)
        {
            Result<Team> retVal = null;
            try
            {
                Team team = Uow.Competitors
                    .GetAll()
                    .OfType<Team>()
                    .FirstOrDefault(p => p.CompetitorID == teamID);

                retVal = ResultHandler<Team>.Sucess(team);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving team, ID: {0}", teamID), ex);
                retVal = ResultHandler<Team>.Erorr("Error retreiving team");
            }

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

        public Result<PagedResult<Player>> GetPlayers(int page, int count)
        {
            Result<PagedResult<Player>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
                                            .Count();

                page = GetPage(totalItems, page, count);
                
                List<Player> players = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
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
                log.Error("Error retreiving players", ex);
                retVal = ResultHandler<PagedResult<Player>>.Erorr("Error retreiving players");
            }

            return retVal;
        }

        public Result<PagedResult<Team>> GetTeams(int page, int count)
        {
            Result<PagedResult<Team>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .OfType<Team>()
                                            .Count();

                page = GetPage(totalItems, page, count);
                
                List<Team> teams = Uow.Competitors
                                            .GetAll()
                                            .OfType<Team>()
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
                log.Error("Error retreiving teams", ex);
                retVal = ResultHandler<PagedResult<Team>>.Erorr("Error retreiving teams");
            }

            return retVal;
        }

        public Result<List<Competitor>> FilterByUserAndCategory(int userID, int gameCategoryID)
        {
            Result<List<Competitor>> retVal = null;
            try
            {
                List<Competitor> allMycompetitors = new List<Competitor>();

                List<Player> players = Uow.Competitors
                                            .GetAll(p => p.Games, p => ((Player)p).Teams)
                                            .OfType<Player>()
                                            .Where(p => p.UserID == userID &&
                                                        p.Games.Any(g => g.Game.GameCategoryID == gameCategoryID))
                                            .OrderBy(p => p.Name)
                                            .ToList();

                foreach (Player player in players)
                {
                    // serialization issue (we dont nee whole user here);
                    player.User = null;
                    allMycompetitors.Add(player);
                    foreach (TeamPlayer teamPlayer in player.Teams)
                    {
                        Team team = (Team)Uow.Competitors.GetById(t => t.CompetitorID == teamPlayer.TeamID, t => t.Games);
                        allMycompetitors.Add(team);
                    }
                }

                // explicitelly load only relevant data
                foreach (Competitor competitor in allMycompetitors)
                {
                    foreach (GameCompetitor gameCompetitor in competitor.Games)
                    {
                        Game game = Uow.Games.GetById(g => g.GameID == gameCompetitor.GameID, g => g.CompetitionTypes);
                        gameCompetitor.Game = new Game()
                        {
                            GameID = game.GameID,
                            GameCategoryID = game.GameCategoryID,
                            Title = game.Title,
                            CompetitionTypes = new List<GameCompetitionType>()
                        };
                        foreach (GameCompetitionType gct in game.CompetitionTypes)
                        {
                            CompetitionType competitonType = Uow.CompetitionTypes.GetById(ct => ct.CompetitionTypeID == gct.CompetitionTypeID);
                            gameCompetitor.Game.CompetitionTypes.Add(new GameCompetitionType()
                            {
                                CompetitionTypeID = competitonType.CompetitionTypeID,
                                CompetitionType = new CompetitionType()
                                {
                                    CompetitionTypeID = competitonType.CompetitionTypeID,
                                    CompetitorType = competitonType.CompetitorType,
                                    Name = competitonType.Name,
                                    CompetitorsCount = competitonType.CompetitorsCount
                                }
                            });
                        }
                    }
                }

                retVal = ResultHandler<List<Competitor>>.Sucess(allMycompetitors);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting list of competitors for user id: {0}, categoryID: {1}", userID, gameCategoryID), ex);
                retVal = ResultHandler<List<Competitor>>.Erorr("Error getting list of competitors");
            }

            return retVal;
        }

        public Result<PagedResult<Competitor>> SearchAndExcludeByCategoryAndCompetitorType(
            int page,
            int count,
            List<long> excludeIds,
            int gameCategoryID,
            CompetitorType competitorType,
            string search)
        {
            Result<PagedResult<Competitor>> retVal = null;
            try
            {
                int totalItems = Uow.GameCompetitors
                    .GetAll(gc => gc.Competitor)
                    .Where(g => !excludeIds.Contains(g.CompetitorID) &&
                                g.Game.GameCategoryID == gameCategoryID &&
                                g.Competitor.CompetitorType == competitorType &&
                                g.Competitor.Name.Contains(search))
                    .Select(g => g.Competitor)
                    .Count();

                page = GetPage(totalItems, page, count);

                List<Competitor> competitors = Uow.GameCompetitors
                    .GetAll(gc => gc.Competitor)
                    .Where(g => !excludeIds.Contains(g.CompetitorID) &&
                                g.Game.GameCategoryID == gameCategoryID &&
                                g.Competitor.CompetitorType == competitorType &&
                                g.Competitor.Name.Contains(search))
                    .OrderBy(g => g.Competitor.Name)
                    .Select(g => g.Competitor)
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
                log.Error(String.Format("Error searching competitors by competition type and category. Competitor type: {0}, game category: {1}",
                    competitorType, gameCategoryID), ex);
                retVal = ResultHandler<PagedResult<Competitor>>.Erorr("Error searching competitors");
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

        public Result<PagedResult<Player>> FilterPlayersByGameCategory(int page, int count, int gameCategoryID)
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

        public Result<PagedResult<Player>> SearchPlayersForGameCategory(int page, int count, int userID, int gameCategoryID, List<long> ids, string search)
        {
            Result<PagedResult<Player>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
                                            .Where(p => p.Games.Any(g => g.Game.Category.GameCategoryID == gameCategoryID) &&
                                                    !ids.Contains(p.CompetitorID) && 
                                                    p.User.UserID != userID &&
                                                    (p.Name.Contains(search) || p.User.FirstName.Contains(search) || p.User.LastName.Contains(search)))
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Player> players = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.Games.Any(g => g.Game.Category.GameCategoryID == gameCategoryID) &&
                                !ids.Contains(p.CompetitorID) && 
                                p.User.UserID != userID &&
                                (p.Name.Contains(search) || p.User.FirstName.Contains(search) || p.User.LastName.Contains(search)))
                    .OrderBy(p => p.User.FirstName)
                    .ThenBy(p => p.User.LastName)
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
                log.Error(String.Format("Error searching players for game category. ID: {0}, search: {1}", gameCategoryID, search), ex);
                retVal = ResultHandler<PagedResult<Player>>.Erorr("Error searching players");
            }

            return retVal;
        }

        public Result<PagedResult<Team>> FilterTeamsByGameCategory(int page, int count, int gameCategoryID)
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

        public Result<PagedResult<Player>> FilterPlayersByTeam(int page, int count, long teamID)
        {
            Result<PagedResult<Player>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
                                            .Where(c => c.Teams.Any(t => t.TeamID == teamID))
                                            .Count();

                List<Player> players = Uow.Competitors
                                            .GetAll()
                                            .OfType<Player>()
                                            .Where(c => c.Teams.Any(t => t.TeamID == teamID))
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
                log.Error(String.Format("Error getting list of players for team. ID: {0}", teamID), ex);
                retVal = ResultHandler<PagedResult<Player>>.Erorr("Error getting list of players for team");
            }

            return retVal;
        }

        public Result<PagedResult<Team>> FilterTeamsByPlayer(int page, int count, long playerID)
        {
            Result<PagedResult<Team>> retVal = null;
            try
            {
                int totalItems = Uow.Competitors
                                            .GetAll()
                                            .OfType<Team>()
                                            .Where(c => c.Players.Any(p => p.PlayerID == playerID))
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Team> teams = Uow.Competitors
                                            .GetAll()
                                            .OfType<Team>()
                                            .Where(c => c.Players.Any(p => p.PlayerID == playerID))
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
                log.Error(String.Format("Error getting list of teams for player. ID: {0}", playerID), ex);
                retVal = ResultHandler<PagedResult<Team>>.Erorr("Error getting list of teams for user");
            }

            return retVal;
        }

        public Result<Player> GetPlayerForGameCategory(int userID, int gameCategoryID)
        {
            Result<Player> retVal = null;
            try
            {
                Player player = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.Games.Any(g => g.Game.GameCategoryID == gameCategoryID) &&
                        p.User.UserID == userID)
                    .FirstOrDefault();

                retVal = ResultHandler<Player>.Sucess(player);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting player for game category. userID: {0}, gameCategoryID: {1}", userID, gameCategoryID), ex);
                retVal = ResultHandler<Player>.Erorr("Error getting player");
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

        public List<long> GetPlayerIdsForTeam(long teamID)
        {
            List<long> retVal = null;
            try
            {
                retVal = Uow.Competitors
                    .GetAll()
                    .OfType<Team>()
                    .Where(t => t.CompetitorID == teamID)
                    .SelectMany(t => t.Players)
                    .Select(p => p.PlayerID)
                    .ToList();
            }
            catch (Exception ex)
            {
                log.Error("Error retrieving player ids", ex);
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

        public Result<Player> GetUpdatePlayer(long playerID)
        {
            Result<Player> retVal = null;
            try
            {
                Player player = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.CompetitorID == playerID)
                    .First();
                
                List<Game> games = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.CompetitorID == playerID)
                    .SelectMany(p => p.Games)
                    .Select(g => g.Game)
                    .ToList();

                games[0].Category = Uow.GameCategories.GetById(games[0].GameCategoryID);

                // assume that there should always be at least one game that player comeptes in
                int gameCategory = games[0].GameCategoryID;
                List<Game> allGames = Uow.Games
                    .GetAll()
                    .Where(g => g.GameCategoryID == gameCategory)
                    .ToList();

                player.Games = new List<GameCompetitor>();

                foreach (Game game in allGames)
                {
                    bool selected = games.FirstOrDefault(gc => gc.GameID == game.GameID) != null;

                    player.Games.Add(new GameCompetitor()
                    {
                        CompetitorID = player.CompetitorID,
                        Competitor = player,
                        Game = game,
                        GameID = game.GameID,
                        Selected = selected
                    });
                }

                player.Games = player.Games.OrderBy(g => g.Game.Title).ToList();

                retVal = ResultHandler<Player>.Sucess(player);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting player for update, ID: {0}", playerID), ex);
                retVal = ResultHandler<Player>.Erorr("Erorr getting player for update");
            }

            return retVal;
        }

        public Result<Team> AddTeam(Team team)
        {
            Result<Team> retVal = null;
            try
            {
                foreach (TeamPlayer tp in team.Players)
                {
                    tp.Player = null;
                }
                team.CompetitorType = CompetitorType.Team;
                team.CreationDate = DateTime.Now;
                Uow.Competitors.Add(team);
                Uow.Commit();

                retVal = ResultHandler<Team>.Sucess(team);
            }
            catch (Exception ex)
            {
                log.Error("Error adding team", ex);
                retVal = ResultHandler<Team>.Erorr("Error adding team");
            }

            return retVal;
        }

        public Result<Team> UpdateTeam(Team team)
        {
            Result<Team> retVal = null;
            try
            {
                List<GameCompetitor> gameCompetitors = Uow.GameCompetitors
                    .GetAll()
                    .Where(gc => gc.CompetitorID == team.CompetitorID)
                    .ToList();
                foreach (GameCompetitor gc in gameCompetitors)
                {
                    Uow.GameCompetitors.Delete(gc);
                }
                foreach (GameCompetitor gc in team.Games.Where(g => g.Selected))
                {
                    gc.Competitor = null;
                    gc.Game = null;
                    Uow.GameCompetitors.Add(gc);
                }
                Uow.Competitors.Update(team, team.CompetitorID);
                Uow.Commit();

                retVal = ResultHandler<Team>.Sucess(team);
            }
            catch (Exception ex)
            {
                log.Error("Error adding team", ex);
                retVal = ResultHandler<Team>.Erorr("Error adding team");
            }

            return retVal;
        }

        public Result<Team> GetUpdateTeam(long teamID, int userID)
        {
            Result<Team> retVal = null;
            try
            {
                Team team = Uow.Competitors
                    .GetAll(c => ((Team)c).Players)
                    .OfType<Team>()
                    .Where(p => p.CompetitorID == teamID)
                    .First();

                List<Game> games = Uow.Competitors
                    .GetAll()
                    .OfType<Team>()
                    .Where(p => p.CompetitorID == teamID)
                    .SelectMany(p => p.Games)
                    .Select(g => g.Game)
                    .ToList();

                games[0].Category = Uow.GameCategories.GetById(games[0].GameCategoryID);

                // assume that there should always be at least one game that player comeptes in
                int gameCategory = games[0].GameCategoryID;
                List<Game> allGames = Uow.Games
                    .GetAll(g => g.Category)
                    .Where(g => g.GameCategoryID == gameCategory)
                    .ToList();

                team.Games = new List<GameCompetitor>();

                foreach (Game game in allGames)
                {
                    bool selected = games.FirstOrDefault(gc => gc.GameID == game.GameID) != null;

                    team.Games.Add(new GameCompetitor()
                    {
                        CompetitorID = team.CompetitorID,
                        Competitor = team,
                        Game = game,
                        GameID = game.GameID,
                        Selected = selected
                    });
                }

                // load players
                foreach (TeamPlayer teamPlayer in team.Players)
                {
                    teamPlayer.Player = Uow.Competitors
                        .GetAll(p => ((Player)p).User)
                        .OfType<Player>()
                        .FirstOrDefault(p => p.CompetitorID == teamPlayer.PlayerID);

                    teamPlayer.Player.IsCurrentUserCompetitor = teamPlayer.Player.UserID == userID;
                    teamPlayer.Player.User = null;
                }
                team.Creator = null;
                team.Games = team.Games.OrderBy(g => g.Game.Title).ToList();

                retVal = ResultHandler<Team>.Sucess(team);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting team for update, ID: {0}", teamID), ex);
                retVal = ResultHandler<Team>.Erorr("Erorr getting team for update");
            }

            return retVal;
        }

        public bool DeleteCompetitor(long competitorID)
        {
            bool retVal = true;
            try
            {
                List<TeamPlayer> players = Uow.TeamPlayers
                    .GetAll()
                    .Where(p => p.TeamID == competitorID)
                    .ToList();
                foreach (TeamPlayer player in players)
                {
                    Uow.TeamPlayers.Delete(player);
                }

                List<GameCompetitor> gameCompetitors = Uow.GameCompetitors
                    .GetAll()
                    .Where(gc => gc.CompetitorID == competitorID)
                    .ToList();
                foreach (GameCompetitor gameCompetitor in gameCompetitors)
                {
                    Uow.GameCompetitors.Delete(gameCompetitor);
                }

                Uow.Competitors.Delete(competitorID);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error("Error deleting competitor", ex);
                retVal = false;
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

        public bool CheckUserCompetitor(int userID, long competitorID)
        {
            bool retVal = false;
            try
            {
                Competitor competitor = Uow.Competitors.GetById(competitorID);
                retVal = (competitor is Player && ((Player)competitor).UserID == userID);
                if (!retVal && competitor is Team)
                {
                    retVal = Uow.Competitors
                        .GetAll()
                        .OfType<Team>()
                        .Where(t => t.CompetitorID == competitorID &&
                            t.Players.Any(p => p.Player.UserID == userID))
                        .Count() > 0;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error checking competitor for user", ex);
            }

            return retVal;
        }

        public bool ConfirmScore(CompetitorScore competitorScore)
        {
            bool retVal = false;
            try 
            {
                Uow.CompetitorScores.Update(competitorScore, competitorScore.CompetitorID, competitorScore.MatchID);
                Uow.Commit();

                bool matchConfirmed = !Uow.CompetitorScores
                    .GetAll()
                    .Any(cs => cs.MatchID == competitorScore.MatchID &&
                               !cs.Confirmed);
                Match match = Uow.Matches.GetById(competitorScore.MatchID);

                if (matchConfirmed)
                {
                    match.Status = MatchStatus.Confirmed;
                    Uow.Matches.Update(match, match.MatchID);
                    Uow.Commit();
                }

                retVal = true;
            }
            catch (Exception ex) 
            {
                log.Error("Error confirming score", ex);
            }

            return retVal;
        }

        public int TotalMatchesCount(long competitorID)
        {
            int retVal = 0;
            try
            {
                retVal = Uow.Matches.GetAll(m => m.Scores.Any(s => s.CompetitorID == competitorID)).Count();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error gettign count of matches for competitor. ID: {0}", competitorID), ex);
            }

            return retVal;
        }
    }
}