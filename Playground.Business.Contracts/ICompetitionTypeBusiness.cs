﻿using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface ICompetitionTypeBusiness
    {
        Result<CompetitionType> GetById(int id);
        Result<List<CompetitionType>> GetAllCompetitionTypes();
        Result<PagedResult<CompetitionType>> GetCompetitionTypes(int page, int count);
        Result<CompetitionType> AddCompetitionType(CompetitionType competitionType);
        Result<CompetitionType> UpdateCompetitionType(CompetitionType competitionType);
        Result<CompetitionType> DeleteCompetitionType(int id);
    }
}
