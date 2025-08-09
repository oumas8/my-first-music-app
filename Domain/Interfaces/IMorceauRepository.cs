using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMorceauRepository:IGenericRepository<Morceau>
    {
        Task<ICollection<Morceau>> _GetPlylisteWithMorceauxAsync(int idPaylist);
    }
}
