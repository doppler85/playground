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

        public GameBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        public Result<Game> AddGameCategory(Game game)
        {
            throw new NotImplementedException();
        }

        public Result<Game> GetById(Game game)
        {
            throw new NotImplementedException();
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