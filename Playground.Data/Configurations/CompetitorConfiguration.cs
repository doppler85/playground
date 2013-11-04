using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class CompetitorConfiguration : EntityTypeConfiguration<Competitor>
    {
        public CompetitorConfiguration()
        {
            // nothing here. necessery for ef 5 table TPH inheritance
            Map(c => 
            {
                c.ToTable("Competitors"); 
            });

            HasKey(c => c.CompetitorID)
                .Property(c => c.CompetitorID)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
        }
    }
}
