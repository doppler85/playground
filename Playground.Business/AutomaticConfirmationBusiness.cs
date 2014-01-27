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

        public Result<AutomaticMatchConfirmation> AddConfirmation(int userID, int confirmeeID)
        {
            Result<AutomaticMatchConfirmation> retVal = null;
            try
            {
                AutomaticMatchConfirmation amc = new AutomaticMatchConfirmation()
                {
                    ConfirmeeID = confirmeeID,
                    ConfirmerID = userID
                };

                Uow.AutomaticMatchConfirmations.Add(amc);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error("Error adding automatic match confrimation", ex);
                retVal = ResultHandler<AutomaticMatchConfirmation>.Erorr("Error adding automatic match confrimation");
            }

            return retVal;
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

        public bool CheckConfirmation(int userID, long competitorID)
        {
            bool retVal = false;
            try
            {
                Competitor competitor = Uow.Competitors.GetById(competitorID);
                if (competitor is Player)
                {
                    int competitorUserid = ((Player)competitor).UserID;
                    AutomaticMatchConfirmation amc = Uow.AutomaticMatchConfirmations
                        .GetAll()
                        .FirstOrDefault(a => a.ConfirmeeID == userID &&
                                             a.ConfirmerID == competitorUserid);

                    retVal = amc != null;
                }
                else
                {
                    List<int> userIds = Uow.TeamPlayers
                        .GetAll(tp => tp.Player)
                        .Where(tp => tp.TeamID == competitorID)
                        .Select(tp => tp.Player.UserID)
                        .ToList();

                    AutomaticMatchConfirmation amc = Uow.AutomaticMatchConfirmations
                        .GetAll()
                        .FirstOrDefault(a => a.ConfirmeeID == userID &&
                                             userIds.Contains(a.ConfirmerID));

                    retVal = amc != null;
                }
            }
            catch (Exception ex)
            {
                log.Error("Erorr checking confrimation", ex);
            }

            return retVal;
        }

        public bool DeleteConfirmation(int confirmeeID, int confirmerID)
        {
            bool retVal = false;
            try
            {
                Uow.AutomaticMatchConfirmations.Delete(ac => ac.ConfirmeeID == confirmeeID &&
                                             ac.ConfirmerID == confirmerID);
                Uow.Commit();

                retVal = true;
            }
            catch (Exception ex)
            {
                log.Error("Error deleting automatic confirmation", ex);
            }

            return retVal;
        }
    }
}
