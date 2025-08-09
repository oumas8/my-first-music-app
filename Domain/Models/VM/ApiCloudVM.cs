using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.VM
{
    public class ApiCloudVM
    {
        public Int64 id { get; set; }
        public String cle { get; set; }
        public String dt_block { get; set; }
        public String type { get; set; }
        public bool fl_blocked { get; set; }
    }
}
