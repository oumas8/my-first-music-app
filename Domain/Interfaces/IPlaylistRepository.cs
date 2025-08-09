using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPlaylistRepository:IGenericRepository<Playlist>
    {
        Task<Playlist> _GetPlylisteWithMorceauxAsync(int id);
        Task<ICollection<Playlist>> GetAllwithMorceau();
        Task<ICollection<Playlist>> GetPlaylistType(int idUser, string type);
        Task<Playlist> _checkPlyliste(int idUser, string name);
    }
}
