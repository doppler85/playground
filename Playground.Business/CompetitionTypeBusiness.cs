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

        public Result<PagedResult<CompetitionType>> GetCompetitionTypes(int page, int count)
        {
            Result<PagedResult<CompetitionType>> retVal = null;
            try
            {
                int totalItems = Uow.CompetitionTypes
                    .GetAll()
                    .Count();

                // check for last page
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

        public Result<CompetitionType> DeleteCompetitionType(int id)
        {
            Result<CompetitionType> retVal = null;
            try
            {
                Uow.CompetitionTypes.Delete(id);
                Uow.Commit();
                retVal = ResultHandler<CompetitionType>.Sucess(null);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error deleting  competition type with following id: {0}", id), ex);
                retVal = ResultHandler<CompetitionType>.Erorr("Error deleting competition type");
            }

            return retVal;
        }
    }
}
