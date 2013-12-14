using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class AutomaticMatchConfirmation
    {
        public int ConfirmeeID { get; set; }
        public int ConfirmerID { get; set; }

        public User Confirmee { get; set; }
        public User Confirmer { get; set; }
    }
}
