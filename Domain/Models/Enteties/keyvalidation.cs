using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Enteties
{
    public class keyvalidation
    {
        [Key]
        public Int64 id { get; set; }
        public String key{ get; set; }
        public Int64 idUser { get; set; }
        public string dtExpired { get; set; }
      
    }
}
