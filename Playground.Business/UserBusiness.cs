using log4net;
using Playground.Business.Contracts;
using Playground.Common;
using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public class UserBusiness : PlaygroundBusinessBase, IUserBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICompetitorBusiness competitorBusiness; 

        public UserBusiness(IPlaygroundUow uow,
            ICompetitorBusiness cBusiness)
        {
            this.Uow = uow;
            this.competitorBusiness = cBusiness;
        }

        public void SetUserPictureUrl(User user)
        {
            if (!String.IsNullOrEmpty(user.PictureUrl))
            {
                user.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
            }
            else
            {
                user.PictureUrl = user.Gender == Gender.Male ?
                    Constants.Images.DefaultProfileMale :
                    Constants.Images.DefaultProfileFemale;
            }
        }

        public Result<User> GetUserByEmail(string email)
        {
            Result<User> retVal = null;
            try
            {
                User user = Uow.Users
                    .GetAll()
                    .FirstOrDefault(u => u.EmailAddress == email);

                SetUserPictureUrl(user);

                retVal = ResultHandler<User>.Sucess(user);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving user by email. email: {0}", email), ex);
                retVal = ResultHandler<User>.Erorr("Error retreiving user by email");
            }

            return retVal;
        }

        public Result<User> GetUserById(int userID)
        {
            Result<User> retVal = null;
            try
            {
                User user = Uow.Users.GetById(userID);

                SetUserPictureUrl(user);

                retVal = ResultHandler<User>.Sucess(user);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving user by ID. ID: {0}", userID), ex);
                retVal = ResultHandler<User>.Erorr("Error retreiving user by email");
            }

            return retVal;
        }

        public Result<User> GetUserByExternalId(string externalUserID)
        {
            Result<User> retVal = null;
            try
            {
                User user = Uow.Users
                    .GetAll()
                    .FirstOrDefault(u => u.ExternalUserID == externalUserID);

                SetUserPictureUrl(user);

                retVal = ResultHandler<User>.Sucess(user);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving user by external ID. ID: {0}", externalUserID), ex);
                retVal = ResultHandler<User>.Erorr("Error retreiving user by email");
            }

            return retVal;
        }

        public Result<PagedResult<Playground.Model.ViewModel.PlaygroundUser>> FilterByPlayground(int page, int count, int playgroundID)
        {
            Result<PagedResult<Playground.Model.ViewModel.PlaygroundUser>> retVal = null;
            try
            {
                int totalItems = Uow.PlaygroundUsers
                    .GetAll()
                    .Where(u => u.PlaygroundID == playgroundID)
                    .Count();

                page = GetPage(totalItems, page, count);

                Playground.Model.Playground playground = Uow.Playgrounds.GetById(playgroundID);

                List<Playground.Model.ViewModel.PlaygroundUser> users = Uow.PlaygroundUsers
                    .GetAll()
                    .Where(u => u.PlaygroundID == playgroundID)
                    .Select(u => u.User)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .Select(u => new Playground.Model.ViewModel.PlaygroundUser() {
                        UserID = u.UserID,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        PictureUrl = u.PictureUrl,
                        IsOwner = u.UserID == playground.OwnerID
                    })
                    .ToList();

                PagedResult<Playground.Model.ViewModel.PlaygroundUser> result = new PagedResult<Playground.Model.ViewModel.PlaygroundUser>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = users
                };

                retVal = ResultHandler<PagedResult<Playground.Model.ViewModel.PlaygroundUser>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error loading users by playgorund. playgroundID: {0}", playgroundID), ex);
                retVal = ResultHandler<PagedResult<Playground.Model.ViewModel.PlaygroundUser>>.Erorr("Error loading users by playgorund");
            }

            return retVal;
        }
                
        public int TotalGamesCount(int userID)
        {
            int retVal = 0;
            try
            {
                retVal = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(c => c.UserID == userID)
                    .SelectMany(c => c.Games)
                    .Select(g => g.Game.GameCategoryID)
                    .Distinct()
                    .Count();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting games count for user. ID: {0}", userID), ex);
            }

            return retVal;
        }

        public int TotalPlayersCount(int userID)
        {
            int retVal = 0;
            try
            {
                retVal = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.UserID == userID)
                    .Count();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting players count for user. ID: {0}", userID), ex);
            }

            return retVal;
        }

        public int TotalTeamsCount(int userID)
        {
            int retVal = 0;
            try
            {
                retVal = Uow.Competitors
                    .GetAll()
                    .OfType<Team>()
                    .Where(t => t.Players.Any(p => p.Player.UserID == userID))
                    .Count();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting teams count for user. ID: {0}", userID), ex);
            }

            return retVal;
        }

        public int TotalMatchesCount(int userID)
        {
            int retVal = 0;
            try
            {
                List<long> competitorIds = competitorBusiness.GetCompetitorIdsForUser(userID);
                retVal = Uow.Matches
                    .GetAll()
                    .Where(m => m.Status == MatchStatus.Confirmed &&
                        m.Scores.Any(s => competitorIds.Contains(s.CompetitorID)))
                    .Distinct()
                    .Count();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting teams count for user. ID: {0}", userID), ex);
            }

            return retVal;
        }
        
        public Result<PagedResult<User>> GetUsers(int page, int count)
        {
            Result<PagedResult<User>> retVal = null;
            try
            {
                int totalItems = Uow.Users
                    .GetAll()
                    .Count();

                page = GetPage(totalItems, page, count);

                List<User> users = Uow.Users
                    .GetAll()
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToList();

                foreach (User user in users)
                {
                    if (!String.IsNullOrEmpty(user.PictureUrl))
                    {
                        user.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
                    }
                    else
                    {
                        user.PictureUrl = user.Gender == Gender.Male ?
                            Constants.Images.DefaultProfileMale :
                            Constants.Images.DefaultProfileFemale;
                    }
                }

                PagedResult<User> result = new PagedResult<User>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = users
                };

                retVal = ResultHandler<PagedResult<User>>.Sucess(result);
            }
            catch(Exception ex)
            {
                log.Error(String.Format("Error retreiving list of users. page: {0}, count: {1}", page, count), ex);
                retVal = ResultHandler<PagedResult<User>>.Erorr("Error retreiving users");
            }

            return retVal;
        }

        public Result<PagedResult<User>> SearchAndExcludeByAutomaticConfirmation(int page, int count, int userID, string search)
        {
            Result<PagedResult<User>> retVal = null;
            try
            {
                int totalItems = Uow.Users
                    .GetAll()
                    .Except(
                        Uow.AutomaticMatchConfirmations.GetAll()
                        .Where(ac => ac.ConfirmerID == userID)
                        .Select(ac => ac.Confirmee))
                    .Where(u => u.UserID != userID &&
                            (u.FirstName.Contains(search) || u.LastName.Contains(search)))
                    .Count();

                page = GetPage(totalItems, page, count);

                List<User> users = Uow.Users
                    .GetAll()
                    .Except(
                        Uow.AutomaticMatchConfirmations.GetAll()
                        .Where(ac => ac.ConfirmerID == userID)
                        .Select(ac => ac.Confirmee))
                    .Where(u => u.UserID != userID &&
                            (u.FirstName.Contains(search) || u.LastName.Contains(search)))
                    .OrderBy(u => u.FirstName)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToList();

                PagedResult<User> result = new PagedResult<User>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = users
                };

                retVal = ResultHandler<PagedResult<User>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of users for automatic confirmation. page: {0}, count: {1}, searhc: {2}", 
                    page, count, search), ex);
                retVal = ResultHandler<PagedResult<User>>.Erorr("Error retreiving users");
            }

            return retVal;
        }

        public Result<PagedResult<User>> SearchAvailableByPlayground(int page, int count, int playgroundID, string search)
        {
            Result<PagedResult<User>> retVal = null;
            try
            {
                List<int> userIds = Uow.PlaygroundUsers
                    .GetAll()
                    .Where(pgu => pgu.PlaygroundID == playgroundID)
                    .Select(u => u.UserID)
                    .ToList();

                int totalItems = Uow.Users
                    .GetAll()
                    .Where(u => !userIds.Contains(u.UserID) &&
                                                     (u.FirstName.Contains(search) ||
                                                      u.LastName.Contains(search)))
                    .Count();

                page = GetPage(totalItems, page, count);

                List<User> users = Uow.Users
                    .GetAll()
                    .Where(u => !userIds.Contains(u.UserID) &&
                                                     (u.FirstName.Contains(search) ||
                                                      u.LastName.Contains(search)))
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToList();

                PagedResult<User> result = new PagedResult<User>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = users
                };

                retVal = ResultHandler<PagedResult<User>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of available users for playground. playgroundID: {0}, search: {1}", 
                    playgroundID, search), ex);
                retVal = ResultHandler<PagedResult<User>>.Erorr("Error retreiving list of available users for playground");
            }

            return retVal;
        }

        public Result<User> AddUser(User user)
        {
            Result<User> retVal = null;
            try
            {
                Uow.Users.Add(user);
                Uow.Commit();

                retVal = ResultHandler<User>.Sucess(user);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error adding user: {0}", user.UserID), ex);
                retVal = ResultHandler<User>.Erorr("Error adding user");
            }

            return retVal;
        }

        public Result<User> UpdateUser(User user)
        {
            Result<User> retVal = null;
            try
            {
                User userToUpdate = Uow.Users.GetById(user.UserID);
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.EmailAddress = user.EmailAddress;
                userToUpdate.Gender = user.Gender;

                Uow.Users.Update(userToUpdate, userToUpdate.UserID);
                Uow.Commit();

                retVal = ResultHandler<User>.Sucess(userToUpdate);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error updating user, userID: {0}", user.UserID), ex);
                retVal = ResultHandler<User>.Erorr("Error updating user");
            }

            return retVal;
        }
    }
}
