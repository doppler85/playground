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
    public class PlaygroundBusiness : PlaygroundBusinessBase, IPlaygroundBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PlaygroundBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        public Result<Playground.Model.Playground> GetById(int playgroundID)
        {
            Result<Playground.Model.Playground> retVal = null;
            try
            {
                Playground.Model.Playground playground = Uow.Playgrounds.GetById(playgroundID);
                retVal = ResultHandler<Playground.Model.Playground>.Sucess(playground);
            }
            catch (Exception ex)
            {
                log.Error("Error getting playground", ex);
                retVal = ResultHandler<Playground.Model.Playground>.Erorr("Error loading playgrond");
            }

            return retVal;
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

        public Result<PagedResult<Playground.Model.Playground>> FilterByUser(int page, int count, int userID)
        {
            throw new NotImplementedException();
        }

        public Result<Playground.Model.Playground> AddPlayground(Playground.Model.Playground playground)
        {
            Result<Playground.Model.Playground> retVal = null;
            try
            {
                playground.CreationDate = DateTime.Now;
                Uow.Playgrounds.Add(playground);
                Uow.Commit();

                PlaygroundUser playgroundUser = new PlaygroundUser()
                {
                    UserID = playground.OwnerID,
                    PlaygroundID = playground.PlaygroundID
                };
                AddUserToPlaygroound(playgroundUser);

                retVal = ResultHandler<Playground.Model.Playground>.Sucess(playground);
            }
            catch (Exception ex)
            {
                log.Error("Erorr adding playgorund", ex);
                retVal = ResultHandler<Playground.Model.Playground>.Erorr("Error adding playground");
            }

            return retVal;
        }

        public Result<Playground.Model.Playground> UpdatePlayground(Playground.Model.Playground playground)
        {
            Result<Playground.Model.Playground> retVal = null;
            try
            {
                Uow.Playgrounds.Update(playground, playground.PlaygroundID);
                Uow.Commit();

                retVal = ResultHandler<Playground.Model.Playground>.Sucess(playground);
            }
            catch (Exception ex)
            {
                log.Error("Error updating playground", ex);
                retVal = ResultHandler<Playground.Model.Playground>.Erorr("Error updating playground");
            }

            return retVal;
        }

        public int TotalPlaygroundsCound()
        {
            int retVal = 0;
            try
            {
                retVal = Uow.Playgrounds.GetAll().Count();
            }
            catch (Exception ex)
            {
                log.Error("Error getting playgrounds count", ex);
            }

            return retVal;
        }

        public bool RemovePlayground(int playgroundID)
        {
            bool retVal = true;
            try
            {
                Playground.Model.Playground playgroundToDelete = Uow.Playgrounds.GetById(p => p.PlaygroundID == playgroundID);
                Uow.PlaygroundUsers.Delete(u => u.UserID == playgroundToDelete.OwnerID && u.PlaygroundID == playgroundID);
                Uow.Playgrounds.Delete(playgroundToDelete);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error("Erorr deleting playgorund", ex);
                retVal = false;
            }
            return retVal;
        }

        public bool AddGameToPlayGround(PlaygroundGame playgroundGame)
        {
            bool retVal = true;
            try
            {
                Uow.PlaygroundGames.Add(playgroundGame);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Erorr adding game to palyground, gameID: {0}, playgroundID: {1}",
                    playgroundGame.GameID,
                    playgroundGame.PlaygroundID), ex);
                retVal = false;
            }

            return retVal;
        }

        public bool RemoveGameFromPlayground(PlaygroundGame playgroundGame)
        {
            bool retVal = true;
            try
            {
                Uow.PlaygroundGames.Delete(playgroundGame);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Erorr delete game from palyground, gameID: {0}, playgroundID: {1}",
                    playgroundGame.GameID,
                    playgroundGame.PlaygroundID), ex);
                retVal = false;
            }

            return retVal;
        }

        public bool AddUserToPlaygroound(PlaygroundUser playgroundUser)
        {
            bool retVal = true;
            try
            {
                Uow.PlaygroundUsers.Add(playgroundUser);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Erorr adding user to palyground, userID: {0}, playgroundID: {1}", 
                    playgroundUser.UserID,
                    playgroundUser.PlaygroundID), ex);
                retVal = false;
            }

            return retVal;
        }

        public bool RemoveUserFromPlayground(PlaygroundUser playgroundUser)
        {
            bool retVal = true;
            try
            {
                Uow.PlaygroundUsers.Delete(playgroundUser);
                Uow.Commit();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Erorr deleting user from palyground, userID: {0}, playgroundID: {1}",
                    playgroundUser.UserID,
                    playgroundUser.PlaygroundID), ex);
                retVal = false;
            }

            return retVal;
        }
    }
}
