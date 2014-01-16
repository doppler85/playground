using log4net;
using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public class GameCategoryBusiness : PlaygroundBusinessBase, IGameCategoryBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GameCategoryBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        private bool CheckExisting(GameCategory gameCategory)
        {
            bool retVal = Uow.GameCategories
                .GetAll()
                .FirstOrDefault(gc => gc.Title == gameCategory.Title && 
                    gc.GameCategoryID != gameCategory.GameCategoryID) != null;

            return retVal;
        }

        public Result<GameCategory> GetById(int id)
        {
            Result<GameCategory> retVal = null;
            try
            {
                GameCategory gameCategory = Uow.GameCategories.GetById(id);
                retVal = ResultHandler<GameCategory>.Sucess(gameCategory);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving game category. id: {0}", id), ex);
                retVal = ResultHandler<GameCategory>.Erorr("Error retreiving game category");
            }

            return retVal;
        }

        public Result<PagedResult<GameCategory>> GetGameCategories(int page, int count)
        {
            Result<PagedResult<GameCategory>> retVal = null;
            try
            {
                int totalItems = Uow.GameCategories
                    .GetAll()
                    .Count();

                // check for last page
                page = GetPage(totalItems, page, count);

                List<GameCategory> categories = Uow.GameCategories.GetAll()
                    .OrderBy(gc => gc.Title)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToList();

                PagedResult<GameCategory> result = new PagedResult<GameCategory>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = categories
                };

                retVal = ResultHandler<PagedResult<GameCategory>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error retreiving list of game categories types. page: {0}, count: {1}", page, count), ex);
                retVal = ResultHandler<PagedResult<GameCategory>>.Erorr("Error retreiving game categories");
            }
            return retVal;
        }

        public Result<GameCategory> AddGameCategory(GameCategory gameCategory)
        {
            Result<GameCategory> retVal = null;
            try
            {
                if (CheckExisting(gameCategory))
                {
                    retVal = ResultHandler<GameCategory>.Erorr("Duplicate game category");
                }
                else
                {
                    Uow.GameCategories.Add(gameCategory);
                    Uow.Commit();
                    retVal = ResultHandler<GameCategory>.Sucess(gameCategory);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error creating game category", ex);
                retVal = ResultHandler<GameCategory>.Erorr("Error creating game category");
            }

            return retVal;
        }


        public Result<GameCategory> UpdateGameCategory(GameCategory gameCategory)
        {
            Result<GameCategory> retVal = null;
            try
            {
                if (CheckExisting(gameCategory))
                {
                    retVal = ResultHandler<GameCategory>.Erorr("Duplicate game category");
                }
                else
                {
                    Uow.GameCategories.Update(gameCategory, gameCategory.GameCategoryID);
                    Uow.Commit();
                    retVal = ResultHandler<GameCategory>.Sucess(gameCategory);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error updating game category", ex);
                retVal = ResultHandler<GameCategory>.Erorr("Error updating game category");
            }

            return retVal;
        }

        public Result<GameCategory> DeleteGameCategory(int id)
        {
            Result<GameCategory> retVal = null;
            try
            {
                Uow.GameCategories.Delete(id);
                Uow.Commit();
                retVal = ResultHandler<GameCategory>.Sucess(null);
            }
            catch (Exception ex)
            {
                log.Error("Error deleting game category", ex);
                retVal = ResultHandler<GameCategory>.Erorr("Error deleting game category");
            }

            return retVal;
        }
    }
}
