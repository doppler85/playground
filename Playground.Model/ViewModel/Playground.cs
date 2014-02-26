using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model.ViewModel
{
    public class Playground
    {
        public int PlaygroundID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public bool IsOwner { get; set; }
        public bool IsMember { get; set; }
    }
}
