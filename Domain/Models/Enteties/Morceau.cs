using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Enteties
{
    public class Morceau
    {
        [Key]
        public Int64 id { get; set; }
        public String title { get; set; }
        public String url { get; set; }
        public String info { get; set; }
        public String morceauID { get; set; }
        [JsonIgnore]
        public bool fl_local { get; set; }
        [JsonIgnore]
        public bool fl_delet { get; set; }
        public ICollection<MorceauPlaylist> MorceauPlaylists { get; set; }
    }
}
