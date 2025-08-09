using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.contract
{
    public interface IPlaylistTask
    {
        Task<long> CreatePlaylist(Playlist _playlist);

        Task<ICollection<Playlist>> GetAllPlaylists();
        Task<ICollection<Playlist>> GetAllPlaylistMorceau();

        Task<Playlist> GetPlaylistById(int _playlistId);

      Task<bool> UpdatePlaylist(Playlist _Playlist);

        Task<bool> DeletePlaylist(int _playlistId);
    }
}
