using Domain.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class ItemsToken
    {
       public List<MorceauVM> MorceauVMs { get; set; }
       public string ContinuationToken { get; set; }

    }
}
