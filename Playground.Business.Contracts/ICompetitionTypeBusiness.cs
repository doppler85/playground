using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface ICompetitionTypeBusiness
    {
        ResponseObject<CompetitionType> CreateCompetitionType(CompetitionType competitionType);
        ResponseObject<CompetitionType> UpdateCompetitionType(CompetitionType competitionType);
        ResponseObject<CompetitionType> DeleteCompetitionType(int id);
    }
}
