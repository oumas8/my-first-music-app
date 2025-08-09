using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.contract
{
    public interface IMorceauPlaylistTask
    {
        Task<bool> CreateMorceauPlaylist(MorceauPlaylist _morceauPlaylist);

        Task<ICollection<MorceauPlaylist>> GetAllMorceauPlaylists();

        Task<MorceauPlaylist> GetMorceauPlaylistById(int _morceauPlaylistId);

        Task<bool> UpdateMorceauPlaylist(MorceauPlaylist _morceauPlaylist);

        Task<bool> DeleteMorceauPlaylist(int _morceauPlaylistId);
    }
}
