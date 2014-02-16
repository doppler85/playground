using log4net;
using Playground.Business.Contracts;
using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public class PlaygroundBusiness : PlaygroundBusinessBase, IPlaygroundBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PlaygroundBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        public Result<PagedResult<Playground.Model.Playground>> GetPlaygrounds(int page, int count, bool all)
        {
            Result<PagedResult<Playground.Model.Playground>> retVal = null;

            try
            {
                int totalItems = all ? Uow.Playgrounds
                        .GetAll()
                        .Count() :
                    Uow.Playgrounds
                        .GetAll()
                        .Where(p => p.Public)
                        .Count();

                page = GetPage(totalItems, page, count);

                var playgrounds = Uow.Playgrounds
                    .GetAll();

                if (!all)
                {
                    playgrounds = playgrounds
                        .Where(p => p.Public);
                }

                playgrounds = playgrounds
                    .OrderByDescending(p => p.CreationDate)
                    .Skip((page - 1) * count)
                    .Take(count);


                PagedResult<Playground.Model.Playground> result = new PagedResult<Playground.Model.Playground>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = playgrounds.ToList()
                };

                retVal = ResultHandler<PagedResult<Playground.Model.Playground>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error getting playgrounds", ex);
                retVal = ResultHandler<PagedResult<Playground.Model.Playground>>.Erorr("Error searching competitors");
            }

            return retVal;
        }
    }
}
