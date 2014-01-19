using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface IMatchBusiness
    {
        Result<PagedResult<Match>> FilterByUser(int page, int count, int userID);

        Result<PagedResult<Match>> FilterByStatus(int page, int count, MatchStatus status);
    }
}
