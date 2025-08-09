using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.VM
{
    public class PlaylistVM
    {
        public Int64 id { get; set; }
        public String name { get; set; }
        public String parametre { get; set; }
        public int count { get; set; } = 0;
    }
}
