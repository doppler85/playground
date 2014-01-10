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

        public ResponseObject<CompetitionType> CreateCompetitionType(CompetitionType competitionType)
        {
            ResponseObject<CompetitionType> retVal = new ResponseObject<CompetitionType>()
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

        public ResponseObject<CompetitionType> UpdateCompetitionType(CompetitionType competitionType)
        {
            ResponseObject<CompetitionType> retVal = new ResponseObject<CompetitionType>()
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

        public ResponseObject<CompetitionType> DeleteCompetitionType(int id)
        {
            throw new NotImplementedException();
        }
    }
}
