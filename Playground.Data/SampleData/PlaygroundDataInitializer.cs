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

        private void AddGames(PlaygroundDbContext context) 
        {
            GameCategory category = context.GameCategories.FirstOrDefault(c => c.Title == "Foozball");

            context.Games.Add(new Game()
            {
                GameCategoryID = category.GameCategoryID,
                Title = "Foozball",
                CompetitionTypes = new List<CompetitionType>()
                {
                    new CompetitionType() {
                        Name = "Single",
                        CompetitorType = Model.CompetitorType.Individual,
                        CompetitorsCount = 2
                    },

                    new CompetitionType() {
                        Name = "Double",
                        CompetitorType = Model.CompetitorType.Team,
                        CompetitorsCount = 2
                    }
                }
            });

            category = context.GameCategories.FirstOrDefault(c => c.Title == "Pool");
            context.Games.Add(new Game()
            {
                GameCategoryID = category.GameCategoryID,
                Title = "Pool",
                CompetitionTypes = new List<CompetitionType>()
                {
                    new CompetitionType() {
                        Name = "Single",
                        CompetitorType = Model.CompetitorType.Individual,
                        CompetitorsCount = 2
                    },

                    new CompetitionType() {
                        Name = "Double",
                        CompetitorType = Model.CompetitorType.Team,
                        CompetitorsCount = 2
                    }
                }
            });
            context.Games.Add(new Game()
            {
                GameCategoryID = category.GameCategoryID,
                Title = "Snooker",
                CompetitionTypes = new List<CompetitionType>()
                {
                    new CompetitionType() {
                        Name = "Single",
                        CompetitorType = Model.CompetitorType.Individual,
                        CompetitorsCount = 2
                    },

                    new CompetitionType() {
                        Name = "Double",
                        CompetitorType = Model.CompetitorType.Team,
                        CompetitorsCount = 2
                    }
                }
            });

            context.SaveChanges();
        }

        private void AddUsers(PlaygroundDbContext context)
        {
            context.Users.Add(new User()
            {
                FirstName = "voja",
                EmailAddress = "voja@gmail.com",
                Gender = Gender.Male,
                LastName = "pankovic"
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
            User userVoja = context.Users.FirstOrDefault(u => u.EmailAddress == "voja@gmail.com");
            User userAlex = context.Users.FirstOrDefault(u => u.EmailAddress == "alex@gmail.com");
            User userNeven = context.Users.FirstOrDefault(u => u.EmailAddress == "neven@gmail.com");
            User userNemanja = context.Users.FirstOrDefault(u => u.EmailAddress == "nemanja@gmail.com");

            Game game = context.Games.FirstOrDefault(g => g.Title == "Foozball");

            context.Competitors.Add(new Player()
            {
                UserID = userVoja.UserID,
                GameID = game.GameID,
                CreationDate = DateTime.Now,
                Name = "Voja Dublefire",
                Status = CompetitorStatus.Active
            });

            context.Competitors.Add(new Player()
            {
                UserID = userAlex.UserID,
                GameID = game.GameID,
                CreationDate = DateTime.Now,
                Name = "Alex Tunderball",
                Status = CompetitorStatus.Active
            });

            context.Competitors.Add(new Player()
            {
                UserID = userNeven.UserID,
                GameID = game.GameID,
                CreationDate = DateTime.Now,
                Name = "Neven Defence Breaker",
                Status = CompetitorStatus.Active
            });

            context.Competitors.Add(new Player()
            {
                UserID = userNemanja.UserID,
                GameID = game.GameID,
                CreationDate = DateTime.Now,
                Name = "Nemanja The Son",
                Status = CompetitorStatus.Active
            });


            context.SaveChanges();
        }

        private void AddTeams(PlaygroundDbContext context)
        {
            Player playerVoja = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "voja@gmail.com" && p.Game.Title == "Foozball");
            Player playerAlex = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "alex@gmail.com" && p.Game.Title == "Foozball");
            Player playerNeven = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "neven@gmail.com" && p.Game.Title == "Foozball");
            Player playerNemanja = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "nemanja@gmail.com" && p.Game.Title == "Foozball");

            Game game = context.Games.FirstOrDefault(g => g.Title == "Foozball");

            Team teamZakivaci = new Team()
            {
                Creator = playerVoja.User,
                GameID = game.GameID,
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
                GameID = game.GameID,
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
        }

        private void AddMatches(PlaygroundDbContext context)
        {
            Player playerVoja = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "voja@gmail.com" && p.Game.Title == "Foozball");
            Player playerAlex = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "alex@gmail.com" && p.Game.Title == "Foozball");
            Player playerNeven = context.Competitors.OfType<Player>().FirstOrDefault(p => p.User.EmailAddress == "neven@gmail.com" && p.Game.Title == "Foozball");

            Team teamFurija = context.Competitors.OfType<Team>().FirstOrDefault(t => t.Name == "Crvena Furija" && t.Game.Title == "Foozball");
            Team teamZakivaci = context.Competitors.OfType<Team>().FirstOrDefault(t => t.Name == "Zabagreli Zakivaci" && t.Game.Title == "Foozball");

            Game game = context.Games.FirstOrDefault(g => g.Title == "Foozball");
            CompetitionType competitionSingle = game.CompetitionTypes.FirstOrDefault(ct => ct.Name == "Single" && ct.CompetitorType == CompetitorType.Individual);
            CompetitionType competitionDouble = game.CompetitionTypes.FirstOrDefault(ct => ct.Name == "Double" && ct.CompetitorType == CompetitorType.Team);

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
