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

        Result<User> GetUserByExternalId(string externalUserID);

        int TotalGamesCount(int userID);

        int TotalPlayersCount(int userID);

        int TotalTeamsCount(int userID);

        int TotalMatchesCount(int userID);

        Result<PagedResult<User>> GetUsers(int page, int count);

        Result<PagedResult<User>> SearchAndExcludeByAutomaticConfirmation(int page, int count, int userID, string search);

        Result<PagedResult<User>> SearchAvailableByPlayground(int page, int count, int playgroundID, string search);

        Result<User> AddUser(User user);

        Result<User> UpdateUser(User user);
    }
}
