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
        void SetUserPictureUrl(User user);

        Result<User> GetUserByEmail(string email);

        Result<User> GetUserById(int userID);

        int TotalGamesCount(int userID);

        int TotalPlayersCount(int userID);

        int TotalTeamsCount(int userID);

        int TotalMatchesCount(int userID);

        Result<PagedResult<User>> GetUsers(int page, int count);
        
        Result<User> UpdateUser(User user);
    }
}
