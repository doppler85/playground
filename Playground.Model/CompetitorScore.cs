using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class CompetitorScore
    {
        public long CompetitorID { get; set; }
        public Competitor Competitor { get; set; }

        public long MatchID { get; set; }
        public Match Match { get; set; }

        public decimal Score { get; set; }
    }
}
