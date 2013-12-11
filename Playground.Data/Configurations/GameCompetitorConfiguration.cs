using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class GameCompetitorConfiguration : EntityTypeConfiguration<GameCompetitor>
    {
        public GameCompetitorConfiguration()
        {
            // game competitor has composite key 
            HasKey(gc => new { gc.GameID, gc.CompetitorID });

            // game competitor has one game, and game has many game competitors
            HasRequired(gc => gc.Game)
                .WithMany(g => g.Competitors)
                .HasForeignKey(gc => gc.GameID)
                .WillCascadeOnDelete(false);

            // game competitor has one competitor, game competitor has many games
            HasRequired(gc => gc.Competitor)
                .WithMany(c => c.Games)
                .HasForeignKey(gc => gc.CompetitorID)
                .WillCascadeOnDelete(false);
        }
    }
}
