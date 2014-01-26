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
    public class AutomaticConfirmationBusiness : PlaygroundBusinessBase, IAutomaticConfirmationBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IUserBusiness userBusiness;

        public AutomaticConfirmationBusiness(IPlaygroundUow uow, 
            IUserBusiness uBusiness)
        {
            this.Uow = uow;
            this.userBusiness = uBusiness;
        }

        public Result<PagedResult<AutomaticMatchConfirmation>> FilterByUser(int page, int count, int userID)
        {
            Result<PagedResult<AutomaticMatchConfirmation>> retVal = null;
            try
            {
                int totalItems = Uow.AutomaticMatchConfirmations
                    .GetAll()
                    .Where(ac => ac.ConfirmerID == userID)
                    .Count();

                page = GetPage(totalItems, page, count);

                List<AutomaticMatchConfirmation> confirmations = Uow.AutomaticMatchConfirmations
                    .GetAll(ac => ac.Confirmee, ac => ac.Confirmer)
                    .Where(ac => ac.ConfirmerID == userID)
                    .OrderBy(ac => ac.Confirmee.FirstName)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToList();

                foreach (AutomaticMatchConfirmation confirmation in confirmations)
                {
                    userBusiness.SetUserPictureUrl(confirmation.Confirmee);
                }

                PagedResult<AutomaticMatchConfirmation> result = new PagedResult<AutomaticMatchConfirmation>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = confirmations
                };

                retVal = ResultHandler<PagedResult<AutomaticMatchConfirmation>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting list of automatic confirmations for user. ID: {0}", userID), ex);
                retVal = ResultHandler<PagedResult<AutomaticMatchConfirmation>>.Erorr("Error getting list of automatic confirmations for user");
            }

            return retVal;
        }
    }
}
