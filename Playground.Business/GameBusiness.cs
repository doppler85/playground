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

        public Result<List<Game>> FilterByCategoryAndCompetitionType(int gameCategoryID, CompetitorType competitorType)
        {
            Result<List<Game>> retVal = null;
            try
            {
                List<Game> games = Uow.GameCompetitionTypes
                    .GetAll()
                    .Where(gc => gc.CompetitionType.CompetitorType == competitorType)
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
    }
}