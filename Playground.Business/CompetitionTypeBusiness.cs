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
    public class CompetitionTypeBusiness : PlaygroundBusinessBase, ICompetitionTypeBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CompetitionTypeBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        public Result<CompetitionType> GetById(int id)
        {
            Result<CompetitionType> retVal = null;
            try
            {
                CompetitionType competitionType = Uow.CompetitionTypes.GetById(id);
                retVal = ResultHandler<CompetitionType>.Sucess(competitionType);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving competition type with following id: {0}", id), ex);
                retVal = ResultHandler<CompetitionType>.Erorr("Error retreiving competition type");
            }

            return retVal;
        }

        public Result<List<CompetitionType>> GetAllCompetitionTypes()
        {
            Result<List<CompetitionType>> retVal = null;
            try
            {
                List<CompetitionType> competitionTypes = Uow.CompetitionTypes
                    .GetAll()
                    .OrderBy(ct => ct.CompetitorType)
                    .ThenBy(ct => ct.Name)
                    .ToList();

                retVal = ResultHandler<List<CompetitionType>>.Sucess(competitionTypes);
            }
            catch (Exception ex)
            {
                log.Error("Error retreiving list of competition types.", ex);
                retVal = ResultHandler<List<CompetitionType>>.Erorr("Error retreiving list of competition types");
            }

            return retVal;
        }

        public Result<List<GameCompetitionType>> FilterByGame(int gameID)
        {
            Result<List<GameCompetitionType>> retVal = null;
            try
            {
                List<CompetitionType> competitionTypes = Uow.CompetitionTypes
                    .GetAll()
                    .ToList();

                List<CompetitionType> gameComeptitionTypes = Uow.Games
                    .GetAll(g => g.CompetitionTypes)
                    .Where(g => g.GameID == gameID)
                    .SelectMany(g => g.CompetitionTypes)
                    .Select(gct => gct.CompetitionType)
                    .ToList();

                List<GameCompetitionType> result = new List<GameCompetitionType>();
                foreach (CompetitionType competitionType in competitionTypes)
                {
                    result.Add(new GameCompetitionType()
                    {
                        GameID = gameID,
                        CompetitionType = competitionType,
                        CompetitionTypeID = competitionType.CompetitionTypeID,
                        Selected = gameComeptitionTypes.Any(gct => gct.CompetitionTypeID == competitionType.CompetitionTypeID)
                    });
                }

                retVal = ResultHandler<List<GameCompetitionType>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of competition types for gameID: {0}", gameID), ex);
                retVal = ResultHandler<List<GameCompetitionType>>.Erorr("Error retreiving list of competition types for game");
            }

            return retVal;
        }

        public Result<List<GameCompetitionType>> FilterByGameAvailable(int gameID)
        {
            Result<List<GameCompetitionType>> retVal = null;
            try
            {
                List<GameCompetitionType> competitionTypes = new List<GameCompetitionType>();
                IQueryable<CompetitionType> availableCompetitionTypes = Uow.CompetitionTypes
                    .GetAll()
                    .Where(ct => !ct.Games.Any(g => g.GameID == gameID))
                    .Distinct();

                foreach (CompetitionType ct in availableCompetitionTypes)
                {
                    competitionTypes.Add(new GameCompetitionType()
                    {
                        CompetitionType = ct,
                        CompetitionTypeID = ct.CompetitionTypeID,
                        GameID = gameID
                    });
                }

                retVal = ResultHandler<List<GameCompetitionType>>.Sucess(competitionTypes);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of available competition types for gameID: {0}", gameID), ex);
                retVal = ResultHandler<List<GameCompetitionType>>.Erorr("Error retreiving list of available competition types for game");
            }

            return retVal;
        }

        public Result<PagedResult<CompetitionType>> GetCompetitionTypes(int page, int count)
        {
            Result<PagedResult<CompetitionType>> retVal = null;
            try
            {
                int totalItems = Uow.CompetitionTypes
                    .GetAll()
                    .Count();
                page = GetPage(totalItems, page, count);

                List<CompetitionType> competitionTypes = Uow.CompetitionTypes
                    .GetAll()
                    .OrderBy(ct => ct.CompetitorType)
                    .ThenBy(ct => ct.Name)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToList();

                PagedResult<CompetitionType> result = new PagedResult<CompetitionType>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = competitionTypes
                };

                retVal = ResultHandler<PagedResult<CompetitionType>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of competition types. page: {0}, count: {1}", page, count), ex);
                retVal = ResultHandler<PagedResult<CompetitionType>>.Erorr("Error retreiving competition types");
            }

            return retVal;
        }

        public Result<CompetitionType> AddCompetitionType(CompetitionType competitionType)
        {
            Result<CompetitionType> retVal = null;
            try
            {
                Uow.CompetitionTypes.Add(competitionType);
                Uow.Commit();
                retVal = ResultHandler<CompetitionType>.Sucess(competitionType);
            }
            catch (Exception ex)
            {
                log.Error("Error creating competition type", ex);
                retVal = ResultHandler<CompetitionType>.Erorr("Error creating competition type");
            }

            return retVal;
        }

        public Result<CompetitionType> UpdateCompetitionType(CompetitionType competitionType)
        {
            Result<CompetitionType> retVal = null;
            try
            {
                Uow.CompetitionTypes.Update(competitionType, competitionType.CompetitionTypeID);
                Uow.Commit();
                retVal = ResultHandler<CompetitionType>.Sucess(competitionType);
            }
            catch (Exception ex)
            {
                log.Error("Error updating competition type", ex);
                retVal = ResultHandler<CompetitionType>.Erorr("Error updating competition type");
            }

            return retVal;
        }

        public bool DeleteCompetitionType(int id)
        {
            bool retVal = true;
            try
            {
                Uow.CompetitionTypes.Delete(id);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error deleting  competition type with following id: {0}", id), ex);
                retVal = false;
            }

            return retVal;
        }
    }
}
