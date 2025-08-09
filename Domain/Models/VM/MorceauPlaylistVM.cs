using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.VM
{
    public class MorceauPlaylistVM
    {
        public Int64 id { get; set; }
        public Int64 idPlaylist { get; set; }
        public Int64 idMorceau { get; set; }
        public Int64 place { get; set; }
        public Int64 fl_Active { get; set; }

    }
}
