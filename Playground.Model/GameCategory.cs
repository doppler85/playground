using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public class GameCategory
    {
        public int GameCategoryID { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        public string PictureUrl { get; set; }


        public virtual List<Game> Games { get; set; }
    }
}
