using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.SampleData
{
    public class PlaygroundDataInitializer : 
        // DropCreateDatabaseAlways<PlaygroundDbContext>
        DropCreateDatabaseIfModelChanges<PlaygroundDbContext>
        // CreateDatabaseIfNotExists<PlaygroundDbContext>
    {
        protected override void Seed(PlaygroundDbContext context)
        {
 	        base.Seed(context);

            AddGameCategories(context);

            AddCompetitionTypes(context);

            AddGames(context);

            AddUsers(context);

            AddPlayers(context);

            AddTeams(context);

            AddMatches(context);
        }

        private void AddGameCategories(PlaygroundDbContext context) 
        {
            context.GameCategories.Add(new GameCategory() 
            {
                Title = "Foozball"
            });

            context.GameCategories.Add(new GameCategory()
            {
                Title = "Pool"
            });

            context.SaveChanges();
        }

        private void AddCompetitionTypes(PlaygroundDbContext context)
        {
            context.CompetitionTypes.Add(new CompetitionType()
            {
                Name = "Single",
                CompetitorType = Model.CompetitorType.Individual,
                CompetitorsCount = 2
            });
            context.CompetitionTypes.Add(new CompetitionType()
            {
                Name = "Double",
                CompetitorType = Model.CompetitorType.Team,
                CompetitorsCount = 2
            });

            context.SaveChanges();
        }

        private void AddGames(PlaygroundDbContext context) 
        {
            GameCategory category = context.GameCategories.FirstOrDefault(c => c.Title == "Foozball");
            CompetitionType singleCompetitionType = context.CompetitionTypes.First(ct => ct.Name == "Single");
            CompetitionType doubleCompetitionType = context.CompetitionTypes.First(ct => ct.Name == "Double");

            Game foozballGame = new Game()
            {
                GameCategoryID = category.GameCategoryID,
                Title = "Foozball",
                CompetitionTypes = new List<GameCompetitionType>()
            };
            context.Games.Add(foozballGame);
            context.SaveChanges();

            foozballGame.CompetitionTypes.Add(new GameCompetitionType() {
                CompetitionTypeID = singleCompetitionType.CompetitionTypeID,
                GameID = foozballGame.GameID
            });
            foozballGame.CompetitionTypes.Add(new GameCompetitionType()
            {
                CompetitionTypeID = doubleCompetitionType.CompetitionTypeID,
                GameID = foozballGame.GameID
            });
            context.SaveChanges();
            // context.Games.Add(foozballGame);

            category = context.GameCategories.FirstOrDefault(c => c.Title == "Pool");
            Game poolGame = new Game()
            {
                GameCategoryID = category.GameCategoryID,
                Title = "Pool",
                CompetitionTypes = new List<GameCompetitionType>()
            };
            context.Games.Add(poolGame);
            context.SaveChanges();

            poolGame.CompetitionTypes.Add(new GameCompetitionType()
            {
                CompetitionTypeID = singleCompetitionType.CompetitionTypeID,
                GameID = poolGame.GameID
            });
            poolGame.CompetitionTypes.Add(new GameCompetitionType()
            {
                CompetitionTypeID = doubleCompetitionType.CompetitionTypeID,
                GameID = poolGame.GameID
            });
       
            context.SaveChanges();
            // context.Games.Add(poolGame);

            Game snookerGame = new Game()
            {
                GameCategoryID = category.GameCategoryID,
                Title = "Snooker",
                CompetitionTypes = new List<GameCompetitionType>()
            };
            context.Games.Add(snookerGame);
            context.SaveChanges();

            snookerGame.CompetitionTypes.Add(new GameCompetitionType()
            {
                CompetitionTypeID = singleCompetitionType.CompetitionTypeID,
                GameID = snookerGame.GameID
            });
            snookerGame.CompetitionTypes.Add(new GameCompetitionType()
            {
                CompetitionTypeID = doubleCompetitionType.CompetitionTypeID,
                GameID = snookerGame.GameID
            });
            context.SaveChanges();
        }

        private void AddUsers(PlaygroundDbContext context)
        {
            context.Users.Add(new User()
            {
                FirstName = "voja",
                EmailAddress = "admin@playground.com",
                Gender = Gender.Male,
                LastName = "pankovic",
                ExternalUserID = 1
            });

            context.Users.Add(new User()
            {
                FirstName = "aleksnadar",
                EmailAddress = "alex@gmail.com",
                Gender = Gender.Male,
                LastName = "gajic"
            });

            context.Users.Add(new User()
            {
                FirstName = "neven",
                EmailAddress = "neven@gmail.com",
                Gender = Gender.Male,
                LastName = "milakara"
            });

            context.Users.Add(new User()
            {
                FirstName = "nemanja",
                EmailAddress = "nemanja@gmail.com",
                Gender = Gender.Male,
                LastName = "janosev"
            });

            context.SaveChanges();
        }

        private void AddPlayers(PlaygroundDbContext context)
        {
            User userVoja = context.Users.FirstOrDefault(u => u.EmailAddress == "admin@playground.com");
            User userAlex = context.Users.FirstOrDefault(u => u.EmailAddress == "alex@gmail.com");
            User userNeven = context.Users.FirstOrDefault(u => u.EmailAddress == "neven@gmail.com");
            User userNemanja = context.Users.FirstOrDefault(u => u.EmailAddress == "nemanja@gmail.com");

            Game game = context.Games.FirstOrDefault(g => g.Title == "Foozball");

            context.Competitors.Add(new Player()
            {
                UserID = userVoja.UserID,
                CompetitorType = Model.CompetitorType.Individual,
                CreationDate = DateTime.Now,
                Name = "Voja Dublefire",
                Status = CompetitorStatus.Active
            });

            context.Competitors.Add(new Player()
            {
                UserID = userAlex.UserID,
                CompetitorType = Model.CompetitorType.Individual,
                CreationDate = DateTime.Now,
                Name = "Alex Tunderball",
                Status = CompetitorStatus.Active
            });

            context.Competitors.Add(new Player()
            {
                UserID = userNeven.UserID,
                CompetitorType = Model.CompetitorType.Individual,
                CreationDate = DateTime.Now,
                Name = "Neven Defence Breaker",
                Status = CompetitorStatus.Active
            });

            context.Competitors.Add(new Player()
            {
                UserID = userNemanja.UserID,
                CompetitorType = Model.CompetitorType.Individual,
                CreationDate = DateTime.Now,
                Name = "Nemanja The Son",
                Status = CompetitorStatus.Active
            });

            context.SaveChanges();

            // add game competitors
            Player playerVoja = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "admin@playground.com");
            Player playerAlex = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "alex@gmail.com");
            Player playerNeven = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "neven@gmail.com");
            Player playerNemanja = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "nemanja@gmail.com");

            context.GameCompetitors.Add(new GameCompetitor()
            {
                GameID = game.GameID,
                CompetitorID = playerVoja.CompetitorID
            });

            context.GameCompetitors.Add(new GameCompetitor()
            {
                GameID = game.GameID,
                CompetitorID = playerAlex.CompetitorID
            });

            context.GameCompetitors.Add(new GameCompetitor()
            {
                GameID = game.GameID,
                CompetitorID = playerNeven.CompetitorID
            });

            context.GameCompetitors.Add(new GameCompetitor()
            {
                GameID = game.GameID,
                CompetitorID = playerNemanja.CompetitorID
            });

            context.SaveChanges();
        }

        private void AddTeams(PlaygroundDbContext context)
        {
            Player playerVoja = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "admin@playground.com");
            Player playerAlex = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "alex@gmail.com");
            Player playerNeven = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "neven@gmail.com");
            Player playerNemanja = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "nemanja@gmail.com");

            Game game = context.Games.FirstOrDefault(g => g.Title == "Foozball");

            Team teamZakivaci = new Team()
            {
                Creator = playerVoja.User,
                CompetitorType = Model.CompetitorType.Team,
                CreationDate = DateTime.Now,
                Name = "Zabagreli Zakivaci",
                Status = CompetitorStatus.Active
            };

            teamZakivaci.Players = new List<TeamPlayer>()
            {
                new TeamPlayer() {
                    Team = teamZakivaci,
                    Player = playerVoja
                },
                new TeamPlayer() {
                    Team = teamZakivaci,
                    Player = playerAlex
                }
            };

            Team teamFurija = new Team()
            {
                Creator = playerNeven.User,
                CompetitorType = Model.CompetitorType.Team,
                CreationDate = DateTime.Now,
                Name = "Crvena Furija",
                Status = CompetitorStatus.Active
            };

            teamFurija.Players = new List<TeamPlayer>()
            {
                new TeamPlayer() {
                    Team = teamFurija,
                    Player = playerNeven
                },
                new TeamPlayer() {
                    Team = teamFurija,
                    Player = playerNemanja
                }
            };

            context.Competitors.Add(teamZakivaci);
            context.Competitors.Add(teamFurija);

            context.SaveChanges();

            // add game competitors
            context.GameCompetitors.Add(new GameCompetitor()
            {
                GameID = game.GameID,
                CompetitorID = teamZakivaci.CompetitorID
            });

            context.GameCompetitors.Add(new GameCompetitor()
            {
                GameID = game.GameID,
                CompetitorID = teamFurija.CompetitorID
            });

            context.SaveChanges();
        }

        private void AddMatches(PlaygroundDbContext context)
        {
            Player playerVoja = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "admin@playground.com");
            Player playerAlex = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "alex@gmail.com");
            Player playerNeven = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "neven@gmail.com");
            
            Team teamFurija = context.Competitors.OfType<Team>().FirstOrDefault(t => t.Name == "Crvena Furija");
            Team teamZakivaci = context.Competitors.OfType<Team>().FirstOrDefault(t => t.Name == "Zabagreli Zakivaci");

            Game game = context.Games.FirstOrDefault(g => g.Title == "Foozball");
            CompetitionType competitionSingle = game.CompetitionTypes.FirstOrDefault(ct => ct.CompetitionType.Name == "Single" && ct.CompetitionType.CompetitorType == CompetitorType.Individual).CompetitionType;
            CompetitionType competitionDouble = game.CompetitionTypes.FirstOrDefault(ct => ct.CompetitionType.Name == "Double" && ct.CompetitionType.CompetitorType == CompetitorType.Team).CompetitionType;

            List<Match> matches = new List<Match>() {
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionSingle,
                    Date = new DateTime(2012, 6, 17, 17, 5, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = playerVoja,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = playerAlex,
                            Score = 5
                        }
                    },
                    Winner = playerVoja
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionSingle,
                    Date = new DateTime(2012, 6, 17, 17, 10, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = playerNeven,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = playerAlex,
                            Score = 5
                        }
                    },
                    Winner = playerNeven
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionSingle,
                    Date = new DateTime(2012, 6, 17, 17, 15, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = playerNeven,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = playerVoja,
                            Score = 5
                        }
                    },
                    Winner = playerNeven
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionSingle,
                    Date = new DateTime(2012, 6, 18, 17, 5, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = playerAlex,
                            Score = 5
                        },
                        new CompetitorScore() {
                            Competitor = playerNeven,
                            Score = 10
                        }
                    },
                    Winner = playerNeven
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionSingle,
                    Date = new DateTime(2012, 6, 18, 17, 10, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = playerVoja,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = playerAlex,
                            Score = 5
                        }
                    },
                    Winner = playerVoja
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionSingle,
                    Date = new DateTime(2012, 6, 18, 17, 15, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = playerVoja,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = playerAlex,
                            Score = 5
                        }
                    },
                    Winner = playerVoja
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionDouble,
                    Date = new DateTime(2012, 10, 07, 17, 5, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = teamZakivaci,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = teamFurija,
                            Score = 5
                        }
                    },
                    Winner = teamZakivaci
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionDouble,
                    Date = new DateTime(2012, 10, 07, 17, 10, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = teamZakivaci,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = teamFurija,
                            Score = 5
                        }
                    },
                    Winner = teamZakivaci
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionDouble,
                    Date = new DateTime(2012, 10, 08, 17, 5, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = teamZakivaci,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = teamFurija,
                            Score = 5
                        }
                    },
                    Winner = teamZakivaci
                },
                new Match()
                {
                    GameID = game.GameID,
                    CompetitionType = competitionDouble,
                    Date = new DateTime(2012, 10, 08, 17, 10, 0),
                    Scores = new List<CompetitorScore>()
                    {
                        new CompetitorScore() {
                            Competitor = teamZakivaci,
                            Score = 10
                        },
                        new CompetitorScore() {
                            Competitor = teamFurija,
                            Score = 5
                        }
                    },
                    Winner = teamZakivaci
                }
            };

            foreach (Match match in matches)
            {
                context.Matches.Add(match);
            }

            context.SaveChanges();
        }
    }
}
