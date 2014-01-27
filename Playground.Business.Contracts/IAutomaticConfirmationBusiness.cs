using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface IAutomaticConfirmationBusiness
    {
        Result<AutomaticMatchConfirmation> AddConfirmation(int userID, int confirmeeID);

        Result<PagedResult<AutomaticMatchConfirmation>> FilterByUser(int page, int count, int userID);

        bool CheckConfirmation(int userID, long competitorID);

        bool DeleteConfirmation(int confirmeeID, int confirmerID);
    }
}
