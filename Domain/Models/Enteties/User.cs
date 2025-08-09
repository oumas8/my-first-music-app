using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Enteties
{
    public class User
    {
        [Key]
        public Int64 id { get; set; }
        public String name { get; set; }
        public String login { get; set; }
        public String password { get; set; }
        public String salt { get; set; }
    }
}
