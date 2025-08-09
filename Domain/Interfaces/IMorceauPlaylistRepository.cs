using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMorceauPlaylistRepository:IGenericRepository<MorceauPlaylist>
    {
        Task<ICollection<Morceau>> _GetListeMorceaubyIdPlylistAsync(string input,int idPaylist,int pageNumber,int pageSize);
        Task<List<MorceauPlaylist>> GetByIdMorceau(int idMorceau);
    }
}
