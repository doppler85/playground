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
    public class GameBusiness : PlaygroundBusinessBase, IGameBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICompetitionTypeBusiness competitionTypeBusiness;

        public GameBusiness(IPlaygroundUow uow, ICompetitionTypeBusiness ctBusiness)
        {
            this.Uow = uow;
            this.competitionTypeBusiness = ctBusiness;
        }

        private bool CheckExisting(Game game)
        {
            bool retVal = Uow.Games
                .GetAll()
                .FirstOrDefault(g => g.Title == game.Title &&
                                     g.GameCategoryID == game.GameCategoryID &&
                                     g.GameID != game.GameID) != null;

            return retVal;
        }

        public Result<Game> GetById(int gameID)
        {
            Result<Game> retVal = null;
            try
            {
                Game game = Uow.Games.GetById(gameID);
                GameCategory category = Uow.GameCategories.GetById(game.GameCategoryID);

                if (!String.IsNullOrEmpty(game.PictureUrl))
                {
                    game.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
                }
                else if (!String.IsNullOrEmpty(category.PictureUrl))
                {
                    game.PictureUrl = String.Format("{0}?nocache={1}", category.PictureUrl, DateTime.Now.Ticks);
                }

                retVal = ResultHandler<Game>.Sucess(game);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving game. ID: {0}", gameID), ex);
                retVal = ResultHandler<Game>.Erorr("Error retreiving game");
            }
            return retVal;
        }

        public int TotalCompetitorsCount(int gameID)
        {
            int retVal = 0;
            try
            {
                retVal = Uow.Competitors
                    .GetAll()
                    .Where(c => c.Games.Any(g => g.GameID == gameID))
                    .Count();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting competitors count for game. ID: {0}", gameID), ex);
            }

            return retVal;
        }

        public int TotalMatchesCount(int gameID)
        {
            int retVal = 0;
            try
            {
                retVal = Uow.Matches
                    .GetAll()
                    .Where(m => m.Scores.Any(s => s.Match.GameID == gameID))
                    .Count();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting competitors count for game, ID: {0}", gameID), ex);
            }

            return retVal;
        }

        public Result<PagedResult<Game>> FilterByCategory(int page, int count, int gameCategoryID)
        {
            Result<PagedResult<Game>> retVal = null;
            try
            {
                int totalItems = Uow.Games
                                            .GetAll()
                                            .Where(g => g.GameCategoryID == gameCategoryID)
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Game> games = Uow.Games
                                            .GetAll()
                                            .Where(g => g.GameCategoryID == gameCategoryID)
                                            .OrderBy(g => g.Title)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                PagedResult<Game> result = new PagedResult<Game>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = games
                };

                retVal = ResultHandler<PagedResult<Game>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of games for category. categoryid: {0}", gameCategoryID), ex);
                retVal = ResultHandler<PagedResult<Game>>.Erorr("Error retreiving list of games for category");
            }
            return retVal;
        }

        public Result<PagedResult<Game>> SearchAvailableByPlayground(int page, int count, int playgroundID, string search)
        {
            Result<PagedResult<Game>> retVal = null;
            try
            {
                List<int> gameIds = Uow.PlaygroundGames
                    .GetAll()
                    .Where(pgg => pgg.PlaygroundID == playgroundID)
                    .Select(g => g.GameID)
                    .ToList();

                int totalItems = Uow.Games
                    .GetAll()
                    .Where(g => !gameIds.Contains(g.GameID) &&
                                                     (g.Title.Contains(search) ||
                                                      g.Category.Title.Contains(search)))
                    .Count();

                page = GetPage(totalItems, page, count);

                List<Game> games = Uow.Games
                    .GetAll()
                    .Where(g => !gameIds.Contains(g.GameID) &&
                                                     (g.Title.Contains(search) ||
                                                      g.Category.Title.Contains(search)))
                    .OrderBy(g => g.Title)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToList();

                PagedResult<Game> result = new PagedResult<Game>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = games
                };

                retVal = ResultHandler<PagedResult<Game>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of available games for playground. playgroundID: {0}, search: {1}", 
                    playgroundID, search), ex);
                retVal = ResultHandler<PagedResult<Game>>.Erorr("Error retreiving list of games for category");
            }

            return retVal;
        }

        public Result<List<Game>> FilterByCategoryAndCompetitionType(int gameCategoryID, CompetitorType competitorType)
        {
            Result<List<Game>> retVal = null;
            try
            {
                List<Game> games = Uow.GameCompetitionTypes
                    .GetAll()
                    .Where(g => g.Game.GameCategoryID == gameCategoryID && 
                           g.CompetitionType.CompetitorType == competitorType)
                    .Select(gc => gc.Game)
                    .ToList();

                retVal = ResultHandler<List<Game>>.Sucess(games);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of games for category and comeptitor type.  categoryid: {0}, competitor type: {1}", gameCategoryID, competitorType), ex);
                retVal = ResultHandler<List<Game>>.Erorr("Error retreiving list of games for category and competitor type");
            }

            return retVal;
        }

        public Result<Game> AddGame(Game game)
        {
            Result<Game> retVal = null;
            try
            {
                if (CheckExisting(game))
                {
                    retVal = ResultHandler<Game>.Erorr("Duplicate game");
                }
                else
                {
                    Uow.Games.Add(game);
                    Uow.Commit();
                    retVal = ResultHandler<Game>.Sucess(game);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error creating game", ex);
                retVal = ResultHandler<Game>.Erorr("Error creating game");
            }

            return retVal;
        }

        public Result<Game> UpdateGame(Game game)
        {
            Result<Game> retVal = null;
            try
            {
                if (CheckExisting(game))
                {
                    retVal = ResultHandler<Game>.Erorr("Duplicate game");
                }
                else
                {
                    List<GameCompetitionType> currentCompetitionTypes = competitionTypeBusiness.FilterByGame(game.GameID).Data;

                    // delete existing competition types for game before adding new ones
                    foreach (GameCompetitionType ct in currentCompetitionTypes.Where(ct => ct.Selected))
                    {
                        Uow.GameCompetitionTypes.Delete(ct);
                    }
                    foreach (GameCompetitionType ct in game.CompetitionTypes.Where(ct => ct.Selected))
                    {
                        ct.Game = null;
                        ct.CompetitionType = null;
                        Uow.GameCompetitionTypes.Add(ct);
                    }
                    game.CompetitionTypes = null;
                    Uow.Games.Update(game, game.GameID);
                    Uow.Commit();

                    retVal = ResultHandler<Game>.Sucess(game);
                }

            }
            catch (Exception ex)
            {
                log.Error("Error updating game", ex);
                retVal = ResultHandler<Game>.Erorr("Error updating game");
            }

            return retVal;
        }

        public bool DeleteGame(int gameID)
        {
            bool retVal = true;
            try
            {
                Uow.Games.Delete(gameID);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error deleting  game with following id: {0}", gameID), ex);
                retVal = false;
            }

            return retVal;
        }

        public void LoadImages(List<Game> games)
        {
            try
            {
                int gmaeCategoryID = games.FirstOrDefault().GameCategoryID;

                GameCategory category = Uow.GameCategories.GetById(gmaeCategoryID);

                string gameCategoryPictureUrl = !String.IsNullOrEmpty(category.PictureUrl) ?
                    category.PictureUrl + String.Format("?nocache={0}", DateTime.Now.Ticks) :
                    String.Empty;

                foreach (Game game in games)
                {
                    if (!String.IsNullOrEmpty(game.PictureUrl))
                    {
                        game.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
                    }
                    else
                    {
                        game.PictureUrl = gameCategoryPictureUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error loading images for games", ex);
            }
        }
    }
}