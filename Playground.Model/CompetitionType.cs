using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public enum CompetitorType
    {
        Individual, 
        Team
    }

    public class CompetitionType
    {
        public int CompetitionTypeID { get; set; }
        public CompetitorType CompetitorType { get; set; }
        public string Name { get; set; }
        public int CompetitorsCount { get; set; }
    }
}
