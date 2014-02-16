using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class PlaygroundUser
    {
        public int UserID { get; set; }
        public int PlaygroundID { get; set; }

        public virtual User User { get; set; }
        public virtual Playground Playground { get; set; }
    }
}
