using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.VM
{
    public class UserVM
    {
        public Int64 id { get; set; }
        public String name { get; set; }
        public String login { get; set; }
        public String password { get; set; }
        public String salt { get; set; }
    }
}
