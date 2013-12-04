using Playground.Model;
using System.Linq;

namespace Playground.Data.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByExternalId(int externalUserID);
        User GetUserByEmail(string email);
    }
}
