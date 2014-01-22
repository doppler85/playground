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

        public UserBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
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
    }
}
