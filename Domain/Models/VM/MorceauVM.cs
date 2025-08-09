using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.VM
{
    public class MorceauVM
    {
        public Int64 id { get; set; }
        public String title { get; set; }
        public String url { get; set; }
        public String info { get; set; }
        public String morceauID { get; set; }
    }
}
