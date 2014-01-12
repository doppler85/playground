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
        Result<CompetitionType> CreateCompetitionType(CompetitionType competitionType);
        Result<CompetitionType> UpdateCompetitionType(CompetitionType competitionType);
        Result<CompetitionType> DeleteCompetitionType(int id);
    }
}
