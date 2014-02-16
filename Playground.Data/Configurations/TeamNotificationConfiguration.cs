using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class TeamNotificationConfiguration : EntityTypeConfiguration<TeamNotification>
    {
        public TeamNotificationConfiguration()
        {
            HasRequired(n => n.Team)
                .WithMany()
                .HasForeignKey(n => n.TeamID)
                .WillCascadeOnDelete(false);
        }
    }
}
