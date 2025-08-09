using Domain.Models.VM;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTO
{
    public class id
    {
        public int valueint { get; set; }
        public string valuestring{ get; set; }
    }
    public class morceauPlylist
    {
        public int idplylist { get; set; }
        public MorceauVM morceauVM { get; set; }
    }
    public class NewPlylist
    {
        public string name { get; set; }
        public string type { get; set; } = "";
        public List<MorceauVM> morceauVMs { get; set; } = null;
    }
}
