using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class PlaygroundGameConfiguration : EntityTypeConfiguration<PlaygroundGame>
    {
        public PlaygroundGameConfiguration()
        {
            // playground game has composite key 
            HasKey(pg => new { pg.PlaygroundID, pg.GameID });

            // playground game has one playground, and palyground has many palyground games
            HasRequired(pg => pg.Playground)
                .WithMany(p => p.Games)
                .HasForeignKey(pg => pg.PlaygroundID)
                .WillCascadeOnDelete(false);

            // playground game has one game, and game has many palygrounds
            HasRequired(pg => pg.Game)
                .WithMany(g => g.Playgrounds)
                .HasForeignKey(pg => pg.GameID)
                .WillCascadeOnDelete(false);
        }
    }
}
