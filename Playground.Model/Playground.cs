using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class Playground
    {
        public int PlaygroundID { get; set; }
        public int OwnerID { get; set; }
        public string Name { get; set; }
        public bool Public { get; set; }
        public string Address { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

        public virtual List<PlaygroundGame> Playgrounds { get; set; }
        public virtual User Owner { get; set; }
        public virtual List<PlaygroundUser> Users { get; set; }
    }
}
