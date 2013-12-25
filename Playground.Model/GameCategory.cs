using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class GameCategory
    {
        public int GameCategoryID { get; set; }
        public string Title { get; set; }
        [NotMapped]
        public string GameCategoryPictureUrl { get; set; }


        public virtual List<Game> Games { get; set; }
    }
}
