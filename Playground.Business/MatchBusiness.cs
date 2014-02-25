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
    public class MatchBusiness : PlaygroundBusinessBase, IMatchBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICompetitorBusiness competitorBusiness;
        private IAutomaticConfirmationBusiness automaticConfirmationsBusiness;

        public MatchBusiness(IPlaygroundUow uow, 
            ICompetitorBusiness cBusiness,
            IAutomaticConfirmationBusiness acBusiness)
        {
            this.Uow = uow;
            this.competitorBusiness = cBusiness;
            this.automaticConfirmationsBusiness = acBusiness;
        }

        public Result<Match> GetMatchById(long matchID)
        {
            Result<Match> retVal = null;
            try
            {
                Match result = Uow.Matches.GetById(matchID);
                retVal = ResultHandler<Match>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error loading match", ex);
                retVal = ResultHandler<Match>.Erorr("Error loading match");
            }

            return retVal;
        }

        public Result<Match> AddMatch(int userID, Match match)
        {
            Result<Match> retVal = null;
            try
            {
                foreach (CompetitorScore competitorScore in match.Scores)
                {
                    if (competitorBusiness.CheckUserCompetitor(userID, competitorScore.CompetitorID))
                    {
                        competitorScore.Confirmed = true;
                    }
                    else if (automaticConfirmationsBusiness.CheckConfirmation(userID, competitorScore.CompetitorID))
                    {
                        competitorScore.Confirmed = true;
                    }
                }

                match.CreatorID = userID;
                match.WinnerID = match.Scores.OrderByDescending(s => s.Score).First().CompetitorID;
                match.Status = match.Scores.Count(s => !s.Confirmed) > 0 ? MatchStatus.Submited : MatchStatus.Confirmed;
                Uow.Matches.Add(match);
                Uow.Commit();

                retVal = ResultHandler<Match>.Sucess(match);
            }
            catch (Exception ex)
            {
                log.Error("Error adding match", ex);
                retVal = ResultHandler<Match>.Erorr("Error adding match");
            }

            return retVal;
        }

        public Result<PagedResult<Match>> FilterByUser(int page, int count, int userID)
        {
            Result<PagedResult<Match>> retVal = null;
            try
            {
                List<long> competitorIds = competitorBusiness.GetCompetitorIdsForUser(userID);

                int totalItems = Uow.Matches
                                            .GetAll()
                                            .Where(m => m.Scores
                                                            .Any(s => competitorIds.Contains(s.CompetitorID)))
                                            .Count();

                page = GetPage(totalItems, page, count);


                List<Match> matches = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.Scores
                                                            .Any(s => competitorIds.Contains(s.CompetitorID)))
                                            .OrderByDescending(s => s.Date)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                foreach (CompetitorScore competitorScore in matches.SelectMany(m => m.Scores))
                {
                    competitorScore.Competitor = Uow.Competitors.GetById(competitorScore.CompetitorID);
                    competitorScore.Competitor.IsCurrentUserCompetitor = competitorIds.Contains(competitorScore.Competitor.CompetitorID);
                }

                PagedResult<Match> result = new PagedResult<Match>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = matches
                };

                retVal = ResultHandler<PagedResult<Match>>.Sucess(result);

            }
            catch (Exception ex)
            {
                log.Error("Error getting list of matches", ex);
                retVal = ResultHandler<PagedResult<Match>>.Erorr("Error getting list of matches");
            }

            return retVal;
        }

        public Result<PagedResult<Match>> FilterByCompetitor(int page, int count, long competitorID)
        {
            Result<PagedResult<Match>> retVal = null;
            try
            {
                int totalItems = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.Status == MatchStatus.Confirmed &&
                                                        m.Scores.Any(s => s.CompetitorID == competitorID))
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Match> matches = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.Status == MatchStatus.Confirmed &&
                                                        m.Scores.Any(s => s.CompetitorID == competitorID))
                                            .OrderByDescending(s => s.Date)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                foreach (CompetitorScore competitorScore in matches.SelectMany(m => m.Scores))
                {
                    competitorScore.Competitor = Uow.Competitors.GetById(competitorScore.CompetitorID);
                }

                PagedResult<Match> result = new PagedResult<Match>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = matches
                };

                retVal = ResultHandler<PagedResult<Match>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error getting list of matches for competitor, ID: {0}", competitorID), ex);
                retVal = ResultHandler<PagedResult<Match>>.Erorr("Error getting list of matches for competitor");
            }

            return retVal;
        }


        public Result<PagedResult<Match>> FilterByStatus(int page, int count, MatchStatus status)
        {
            Result<PagedResult<Match>> retVal = null;
            try
            {
                int totalItems = Uow.Matches
                            .GetAll()
                            .Where(m => m.Status == status)
                            .Count();

                page = GetPage(totalItems, page, count);

                List<Match> matches = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.Status == status)
                                            .OrderByDescending(s => s.Date)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                foreach (CompetitorScore competitorScore in matches.SelectMany(m => m.Scores)) 
                {
                    competitorScore.Competitor = Uow.Competitors.GetById(competitorScore.CompetitorID);
                }

                PagedResult<Match> result = new PagedResult<Match>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = matches
                };

                retVal = ResultHandler<PagedResult<Match>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error getting list of matches", ex);
                retVal = ResultHandler<PagedResult<Match>>.Erorr("Error getting list of matches");
            }

            return retVal;
        }

        public Result<PagedResult<Match>> FilterByStatusAndUser(int page, int count, MatchStatus status, int userID)
        {
            Result<PagedResult<Match>> retVal = null;
            try
            {
                List<long> competitorIds = competitorBusiness.GetCompetitorIdsForUser(userID);

                int totalItems = Uow.Matches
                                            .GetAll()
                                            .Where(m => m.Status == status &&
                                                        m.Scores
                                                            .Any(s => competitorIds.Contains(s.CompetitorID)))
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Match> matches = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.Status == status &&
                                                        m.Scores
                                                            .Any(s => competitorIds.Contains(s.CompetitorID)))
                                            .OrderByDescending(s => s.Date)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                foreach (CompetitorScore competitorScore in matches.SelectMany(m => m.Scores))
                {
                    competitorScore.Competitor = Uow.Competitors.GetById(competitorScore.CompetitorID);
                }

                PagedResult<Match> result = new PagedResult<Match>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = matches
                };

                retVal = ResultHandler<PagedResult<Match>>.Sucess(result);

            }
            catch (Exception ex)
            {
                log.Error("Error getting list of matches", ex);
                retVal = ResultHandler<PagedResult<Match>>.Erorr("Error getting list of matches");
            }

            return retVal;
        }

        public Result<PagedResult<Match>> FilterByStatusAndGame(int page, int count, MatchStatus status, int gameID)
        {
            Result<PagedResult<Match>> retVal = null;
            try
            {
                int totalItems = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.GameID == gameID && m.Status == status)
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Match> matches = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.GameID == gameID && m.Status == status)
                                            .OrderByDescending(s => s.Date)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                foreach (CompetitorScore competitorScore in matches.SelectMany(m => m.Scores))
                {
                    competitorScore.Competitor = Uow.Competitors.GetById(competitorScore.CompetitorID);
                }

                PagedResult<Match> result = new PagedResult<Match>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = matches
                };

                retVal = ResultHandler<PagedResult<Match>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error getting list of matches", ex);
                retVal = ResultHandler<PagedResult<Match>>.Erorr("Error getting list of matches");
            }

            return retVal;
        }

        public Result<PagedResult<Match>> FilterByStatusAndGameCategory(int page, int count, MatchStatus status, int gameCategoryID)
        {
            Result<PagedResult<Match>> retVal = null;
            try
            {
                int totalItems = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.Game.GameCategoryID == gameCategoryID && m.Status == status)
                                            .Count();

                page = GetPage(totalItems, page, count);

                List<Match> matches = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.Game.GameCategoryID == gameCategoryID && m.Status == status)
                                            .OrderByDescending(s => s.Date)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                foreach (CompetitorScore competitorScore in matches.SelectMany(m => m.Scores))
                {
                    competitorScore.Competitor = Uow.Competitors.GetById(competitorScore.CompetitorID);
                }

                PagedResult<Match> result = new PagedResult<Match>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = matches
                };

                retVal = ResultHandler<PagedResult<Match>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error getting list of matches", ex);
                retVal = ResultHandler<PagedResult<Match>>.Erorr("Error getting list of matches");
            }

            return retVal;
        }

        public int TotalMatchesByStatus(MatchStatus status)
        {
            int retVal = 0;
            try
            {
                retVal = Uow.Matches.GetAll().Where(m => m.Status == status).Count();
            }
            catch (Exception ex)
            {
                log.Error("Error gettign matches count", ex);
            }

            return retVal;
        }

        public void LoadScores(Match match)
        {
            try
            {
                List<CompetitorScore> scores = Uow.CompetitorScores
                    .GetAll(s => s.Competitor)
                    .Where(s => s.MatchID == match.MatchID)
                    .ToList();

                match.Scores = scores;
            }
            catch (Exception ex)
            {
                log.Error("Error loading scores for match ", ex);
            }
        }

    }
}
