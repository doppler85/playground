using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class MatchConfiguration : EntityTypeConfiguration<Match>
    {
        public MatchConfiguration()
        {
            HasRequired(m => m.Winner)
                .WithMany()
                .HasForeignKey(m => m.WinnerID)
                .WillCascadeOnDelete(false);
        }
    }
}
