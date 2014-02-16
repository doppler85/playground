using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class PlaygroundGame
    {
        public int PlaygroundID { get; set; }
        public int GameID { get; set; }

        public virtual Playground Playground { get; set; }
        public virtual Game Game { get; set; } 
    }
}
