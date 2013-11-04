using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class AutomaticMatchConfirmationConfiguration : EntityTypeConfiguration<AutomaticMatchConfirmation>
    {
        public AutomaticMatchConfirmationConfiguration()
        {
            // AutomaticMatchConfirmation has composite key 
            HasKey(ac => new { ac.ConfirmeeID, ac.ConfirmerID});

            HasRequired(ac => ac.Confirmee)
                .WithMany()
                .HasForeignKey(ac => ac.ConfirmeeID)
                .WillCascadeOnDelete(false);

            HasRequired(ac => ac.Confirmer)
                .WithMany()
                .HasForeignKey(ac => ac.ConfirmerID)
                .WillCascadeOnDelete(false);
        }
    }
}
