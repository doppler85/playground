using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Configurations
{
    public class PlayerConfiguration : EntityTypeConfiguration<Player>
    {
        public PlayerConfiguration()
        {
            // nothing here. necessery for ef 5 table TPH inheritance
            Map(p => 
                { 
                    p.MapInheritedProperties();
                    p.Requires("CompetitorType").HasValue(1);
                });

            HasKey(p => p.CompetitorID);
        }
    }
}
