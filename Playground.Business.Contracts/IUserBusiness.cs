using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface IUserBusiness
    {
        Result<User> GetUserByEmail(string email);
        
        Result<PagedResult<User>> GetUsers(int page, int count);
    }
}
