using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class AutomaticMatchConfirmation
    {
        public long ConfirmeeID { get; set; }
        public long ConfirmerID { get; set; }

        public Competitor Confirmee { get; set; }
        public Competitor Confirmer { get; set; }
    }
}
