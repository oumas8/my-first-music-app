using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Enteties
{
    public class Playlist
    {
        [Key]
        public Int64 id { get; set; }
        public Int64 idUser { get; set; }
        public bool flActive { get; set; }
        public bool flshow { get; set; }
        public String name { get; set; }
        public String parametre { get; set; }
        public ICollection<MorceauPlaylist> MorceauPlaylists { get; set; }
    }
}
