using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class GameCompetitionTypeConfiguration : EntityTypeConfiguration<GameCompetitionType>
    {
        public GameCompetitionTypeConfiguration()
        {
            HasKey(gct => new { gct.GameID, gct.CompetitionTypeID });

            Map(c =>
            {
                c.ToTable("GameCompetitionTypes");
            });

            HasRequired(gct => gct.Game)
                .WithMany(g => g.CompetitionTypes)
                .HasForeignKey(g => g.GameID)
                .WillCascadeOnDelete(false);

            HasRequired(gct => gct.CompetitionType)
                .WithMany(ct => ct.Games)
                .HasForeignKey(ct => ct.CompetitionTypeID)
                .WillCascadeOnDelete(false);

        }
    }
}
