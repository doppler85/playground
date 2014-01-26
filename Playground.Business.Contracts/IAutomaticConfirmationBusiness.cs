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
        Result<PagedResult<AutomaticMatchConfirmation>> FilterByUser(int page, int count, int userID);
    }
}
