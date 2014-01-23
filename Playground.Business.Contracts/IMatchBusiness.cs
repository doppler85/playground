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

        Result<PagedResult<Match>> FilterByStatusAndUser(int page, int count, MatchStatus status, int userID);

        Result<PagedResult<Match>> FilterByStatusAndGame(int page, int count, MatchStatus status, int gameID);

        Result<PagedResult<Match>> FilterByStatusAndGameCategory(int page, int count, MatchStatus status, int gameCategoryID);
    }
}
