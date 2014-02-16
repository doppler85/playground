using Playground.Data.Configurations;
using Playground.Data.SampleData;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data
{
    public class PlaygroundDbContext : DbContext
    {
        public PlaygroundDbContext()
            : base(nameOrConnectionString: "PlayGround") { 
        }

        static PlaygroundDbContext()
        {
           // Database.SetInitializer(new PlaygroundDataInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CompetitorConfiguration());
            modelBuilder.Configurations.Add(new PlayerConfiguration());
            modelBuilder.Configurations.Add(new TeamConfiguration());
            modelBuilder.Configurations.Add(new CompetitorScoreConfiguration());
            modelBuilder.Configurations.Add(new AutomaticMatchConfirmationConfiguration());
            modelBuilder.Configurations.Add(new TeamPlayerConfiguration());
            modelBuilder.Configurations.Add(new MatchConfiguration());
            modelBuilder.Configurations.Add(new GameCompetitionTypeConfiguration());
            modelBuilder.Configurations.Add(new GameCompetitorConfiguration());
        }


        public DbSet<GameCategory> GameCategories { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<CompetitionType> CompetitionTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Competitor> Competitors { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<AutomaticMatchConfirmation> AutomaticMatchConfirmations { get; set; }
        public DbSet<CompetitorScore> CompetitorScores { get; set; }
        public DbSet<TeamPlayer> TeamPlayers { get; set; }
        public DbSet<GameCompetitor> GameCompetitors { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
