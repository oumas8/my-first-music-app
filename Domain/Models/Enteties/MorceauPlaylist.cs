using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Enteties
{
    public class MorceauPlaylist
    {
        [Key]
        public Int64 id { get; set; }
        public Int64 idPlaylist { get; set; }
        public Int64 idMorceau { get; set; }
        public Int64 place { get; set; }
        public bool fl_Active { get; set; }
        public string dt_creat { get; set; }
        public DateTime dt_listning { get; set; }
        [ForeignKey("idMorceau")]
        public virtual Morceau Morceau { get; set; }
        [ForeignKey("idPlaylist")]
        public virtual Playlist Playlist { get; set; }

    }
}
