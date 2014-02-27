using log4net;
using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using Playground.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public Result<Playground.Model.ViewModel.Playground> GetById(int playgroundID, int userID)
        {
            Result<Playground.Model.ViewModel.Playground> retVal = null;
            try
            {
                Playground.Model.Playground playground = Uow.Playgrounds.GetById(playgroundID);
                bool isOwner = playground.OwnerID == userID;
                bool isMember = Uow.PlaygroundUsers.GetAll().Any(pgu => pgu.PlaygroundID == playgroundID && pgu.UserID == userID);

                Playground.Model.ViewModel.Playground playgorundVM = new Model.ViewModel.Playground()
                {
                    PlaygroundID = playground.PlaygroundID,
                    Address = playground.Address,
                    Latitude = playground.Latitude,
                    Longitude = playground.Longitude,
                    Name = playground.Name,
                    IsMember = isMember,
                    IsOwner = isOwner
                };

                retVal = ResultHandler<Playground.Model.ViewModel.Playground>.Sucess(playgorundVM);
            }
            catch (Exception ex)
            {
                log.Error("Error getting playground", ex);
                retVal = ResultHandler<Playground.Model.ViewModel.Playground>.Erorr("Error loading playgrond");
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

        public Result<List<Playground.Model.Playground>> SearchInArea(Location startLocation, Location endLocation, int maxResults)
        {
            Result<List<Playground.Model.Playground>> retVal = null;
            try
            {
                List<Playground.Model.Playground> result = Uow.PlaygroundUsers
                    .GetAll()
                    .Where(pgu => pgu.Playground.Latitude >= startLocation.Latitude &&
                        pgu.Playground.Latitude <= endLocation.Latitude &&
                        pgu.Playground.Longitude >= startLocation.Longitude &&
                        pgu.Playground.Longitude <= endLocation.Longitude)
                    .GroupBy(g => g.Playground)
                    .Select(pgr => new { Playground = pgr.Key, UserCount = pgr.Count() })
                    .OrderByDescending(pgr => pgr.UserCount)
                    .Take(maxResults)
                    .Select(pgr => pgr.Playground)
                    .ToList();

                retVal = ResultHandler<List<Playground.Model.Playground>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error searching playgrounds", ex);
                retVal = ResultHandler<List<Playground.Model.Playground>>.Erorr("Error searching playgrounds");
            }

            return retVal;
        }

        public Result<PagedResult<Playground.Model.ViewModel.Playground>> SearchPlaygrounds(SearchAreaArgs args, int userID)
        {
            Result<PagedResult<Playground.Model.ViewModel.Playground>> retVal = null;
            try
            {
                if (args.Search == null)
                {
                    args.Search = String.Empty;
                }

                Expression<Func<Playground.Model.Playground, bool>> where = null;
                if (args.GlobalSearch)
                {
                    where = pg => pg.Name.Contains(args.Search) ||
                                pg.Games.Any(g => g.Game.Title.Contains(args.Search) ||
                                             g.Game.Category.Title.Contains(args.Search));
                }
                else
                {
                    where = pg => (pg.Latitude >= args.StartLocationLatitude &&
                                    pg.Latitude <= args.EndLocationLatitude &&
                                    pg.Longitude >= args.StartLocationLongitude &&
                                    pg.Longitude <= args.EndLocationLongitude) &&
                                  (pg.Name.Contains(args.Search) ||
                                                    pg.Games.Any(g => g.Game.Title.Contains(args.Search) ||
                                                    g.Game.Category.Title.Contains(args.Search)));
                }


                int totalItems = Uow.Playgrounds
                                            .GetAll()
                                            .Where(where)
                                            .Count();

                args.Page = GetPage(totalItems, args.Page, args.Count);

                List<Playground.Model.Playground> playgrounds = Uow.Playgrounds
                                            .GetAll()
                                            .Where(where)
                                            .OrderBy(pg => pg.Name)
                                            .Skip((args.Page - 1) * args.Count)
                                            .Take(args.Count)
                                            .ToList();

                List<Playground.Model.ViewModel.Playground> pgs = new List<Model.ViewModel.Playground>();
                
                foreach (Playground.Model.Playground playground in playgrounds) 
                {
                    bool isOwner = userID == playground.OwnerID;
                    bool isMember = Uow.PlaygroundUsers.GetAll().Any(pgu => pgu.PlaygroundID == playground.PlaygroundID &&
                        pgu.UserID == userID);

                    pgs.Add(new Model.ViewModel.Playground()
                    {
                        PlaygroundID = playground.PlaygroundID,
                        Name = playground.Name,
                        Address = playground.Address,
                        Latitude = playground.Latitude,
                        Longitude = playground.Longitude,
                        IsMember = isMember,
                        IsOwner = isOwner
                    });
                }

                PagedResult<Playground.Model.ViewModel.Playground> result = new PagedResult<Playground.Model.ViewModel.Playground>()
                {
                    CurrentPage = args.Page,
                    TotalPages = (totalItems + args.Count - 1) / args.Count,
                    TotalItems = totalItems,
                    Items = pgs
                };

                retVal = ResultHandler<PagedResult<Playground.Model.ViewModel.Playground>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error searching playgrounds", ex);
                retVal = ResultHandler<PagedResult<Playground.Model.ViewModel.Playground>>.Erorr("Error searching playgrounds");
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

        public Result<Playground.Model.ViewModel.PlaygroundStats> GetStats(int playgroundID)
        {
            Result<Playground.Model.ViewModel.PlaygroundStats> retVal = null;
            try
            {
                int totalGames = Uow.PlaygroundGames
                    .GetAll()
                    .Where(pgg => pgg.PlaygroundID == playgroundID)
                    .Count();

                int totalUsers = Uow.PlaygroundUsers
                    .GetAll()
                    .Where(pgu => pgu.PlaygroundID == playgroundID)
                    .Count();
                
                // TODO : make intercepting collection of user games and playground games
                // only show players that play gmes of the playground
                int totalPlayers = Uow.PlaygroundUsers
                    .GetAll()
                    .Where(pgu => pgu.PlaygroundID == playgroundID)
                    .SelectMany(p => p.User.PlayerProfiles)
                    .Count();

                int totalTeams = Uow.PlaygroundUsers
                                    .GetAll()
                                    .Where(pgu => pgu.PlaygroundID == playgroundID)
                                    .SelectMany(p => p.User.PlayerProfiles)
                                    .SelectMany(p => p.Teams)
                                    .Distinct()
                                    .Count();

                int totalMatches = Uow.Matches
                    .GetAll()
                    .Where(m => m.PlaygroundID == playgroundID)
                    .Count();

                Playground.Model.ViewModel.PlaygroundStats result = new Model.ViewModel.PlaygroundStats()
                {
                    TotalGames = totalGames,
                    TotalUsers = totalUsers,
                    TotalPlayers = totalPlayers + totalTeams,
                    TotalMatches = totalMatches
                };

                retVal = ResultHandler<Playground.Model.ViewModel.PlaygroundStats>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Erorr loading playground stats. PlaygroundID : {0}",playgroundID), ex);
                retVal = ResultHandler < Playground.Model.ViewModel.PlaygroundStats>.Erorr("Erorr loading playground stats.");
            }

            return retVal;
        }
    }
}
