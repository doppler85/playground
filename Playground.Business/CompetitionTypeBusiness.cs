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
        public CompetitionTypeBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        public Result<CompetitionType> CreateCompetitionType(CompetitionType competitionType)
        {
            Result<CompetitionType> retVal = new Result<CompetitionType>()
            {
                Sucess = true
            };
            try
            {
                Uow.CompetitionTypes.Add(competitionType);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                // add some logging functionlaity here
                retVal.Data = null;
                retVal.Sucess = false;
                retVal.Message = "Error adding competition type";
            }
            return retVal;
        }

        public Result<CompetitionType> UpdateCompetitionType(CompetitionType competitionType)
        {
            Result<CompetitionType> retVal = new Result<CompetitionType>()
            {
                Sucess = true
            };
            try
            {
                Uow.CompetitionTypes.Update(competitionType, competitionType.CompetitionTypeID);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                // add some logging functionlaity here
                retVal.Data = null;
                retVal.Sucess = false;
                retVal.Message = "Error updating competition type";
            }
            return retVal;
        }

        public Result<CompetitionType> DeleteCompetitionType(int id)
        {
            throw new NotImplementedException();
        }
    }
}
