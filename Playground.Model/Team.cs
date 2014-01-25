using Playground.Model.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class Team : Competitor
    {
        public int CreatorID { get; set; }
        public virtual User Creator { get; set; }
        
        public virtual List<TeamPlayer> Players { get; set; }
    }
}
