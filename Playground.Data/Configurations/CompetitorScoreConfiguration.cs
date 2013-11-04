using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class CompetitorScoreConfiguration : EntityTypeConfiguration<CompetitorScore>
    {
        public CompetitorScoreConfiguration()
        {
            // competitor score has composite key 
            HasKey(cs => new { cs.CompetitorID, cs.MatchID });

            // competitor score has one match, and match has many competitor scores
            HasRequired(cs => cs.Match)
                .WithMany(m => m.Scores)
                .HasForeignKey(cs => cs.MatchID)
                .WillCascadeOnDelete(false);

            // competitor score has one competitor, and competitor has many competotorscores
            HasRequired(cs => cs.Competitor)
                .WithMany(c => c.Scores)
                .HasForeignKey(cs => cs.CompetitorID)
                .WillCascadeOnDelete(false);
        }
    }
}
