using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class TeamPlayerConfiguration : EntityTypeConfiguration<TeamPlayer>
    {
        public TeamPlayerConfiguration()
        {
            HasKey(tp => new { tp.TeamID, tp.PlayerID });

            Map(c =>
            {
                c.ToTable("TeamPlayers");
            });

            HasRequired(tp => tp.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(tp => tp.TeamID)
                .WillCascadeOnDelete(false);

            HasRequired(tp => tp.Player)
                .WithMany(p => p.Teams)
                .HasForeignKey(tp => tp.PlayerID)
                .WillCascadeOnDelete(false);
        }
    }
}
