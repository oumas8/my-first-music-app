using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Enteties
{
    public class Setting
    {
        [Key]
        public Int64 id { get; set; }
        public Int64 idUser { get; set; }
        public String jsonSetting { get; set; }
       
    }
}
