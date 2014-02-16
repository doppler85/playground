using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class PlaygroundUserConfiguration : EntityTypeConfiguration<PlaygroundUser>
    {
        public PlaygroundUserConfiguration()
        {
            // playground game has composite key 
            HasKey(pg => new { pg.PlaygroundID, pg.UserID });

            // playground game has one playground, and palyground has many palyground games
            HasRequired(pg => pg.Playground)
                .WithMany(p => p.Users)
                .HasForeignKey(pg => pg.PlaygroundID)
                .WillCascadeOnDelete(false);

            // playground game has one game, and game has many palygrounds
            HasRequired(pg => pg.User)
                .WithMany(u => u.Playgrounds)
                .HasForeignKey(pg => pg.UserID)
                .WillCascadeOnDelete(false);
        }
    }
}
