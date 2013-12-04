using Playground.Data.Contracts;
using Playground.Model;
using System.Data.Entity;
using System.Linq;

namespace Playground.Data
{
    public class UserRepository : EFRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context) { }

        public User GetUserByExternalId(int externalUserID)
        {
            return DbSet.FirstOrDefault(u => u.ExternalUserID == externalUserID);
        }

        public User GetUserByEmail(string email)
        {
            return DbSet.FirstOrDefault(u => u.EmailAddress == email);
        }
    }
}
