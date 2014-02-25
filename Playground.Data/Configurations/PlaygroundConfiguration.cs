using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class PlaygroundConfiguration : EntityTypeConfiguration<Playground.Model.Playground>
    {
        public PlaygroundConfiguration()
        {
            HasRequired(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerID)
                .WillCascadeOnDelete(false);

            Property(p => p.Longitude).HasPrecision(18, 15);
            Property(p => p.Latitude).HasPrecision(18, 15);
        }
    }
}
