using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasks.contract;

namespace Tasks
{
    public class PlaylistTask: IPlaylistTask
    {
        public IUnitOfWork _unitOfWork;

        public PlaylistTask(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
   
        public async Task<long> CreatePlaylist(Playlist _playlist)
        {
            if (_playlist != null)
            {
                await _unitOfWork.PlaylistRepo.Add(_playlist);

                var result = _unitOfWork.Save();

                    return _playlist.id;
                
            }
            return 0;
        }
        public async Task<bool> DeletePlaylist(int _morceauId)
        {
            if (_morceauId > 0)
            {
                var _morceaus = await _unitOfWork.PlaylistRepo.GetById(_morceauId);
                if (_morceaus != null)
                {
                    _unitOfWork.PlaylistRepo.Delete(_morceaus);
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<ICollection<Playlist>> GetAllPlaylists()
        {
            var morceauList = await _unitOfWork.PlaylistRepo.GetAll();
            return morceauList;
        }
        public async Task<Playlist> CheckPlylistAsync(int idUser,string name)
        {
            var playlist = await _unitOfWork.PlaylistRepo._checkPlyliste(idUser, name);
            return playlist;
        }
        public async Task<ICollection<Playlist>> GetAllPlaylistMorceau()
        {
            var morceauList = await _unitOfWork.PlaylistRepo.GetAllwithMorceau();
            return morceauList;
        }
        public async Task<ICollection<Playlist>> GetPlaylistMorceauType(int idUser,string Type)
        {
            var morceauList = await _unitOfWork.PlaylistRepo.GetPlaylistType(idUser, Type);
            return morceauList;
        }

        public async Task<Playlist> GetPlaylistById(int _morceauId)
        {
            if (_morceauId > 0)
            {
                var _playlist = await _unitOfWork.PlaylistRepo.GetById(_morceauId);
                if (_playlist != null)
                        return _playlist;
                
            }
            return null;
        }

        public async Task<bool> UpdatePlaylist(Playlist _playlist)
        {
            if (_playlist != null)
            {
                var _oldPlaylist = await _unitOfWork.PlaylistRepo.GetById((int)_playlist.id);
                if (_oldPlaylist != null)
                {
                    _oldPlaylist.name = _playlist.name;
                    _oldPlaylist.parametre = _playlist.parametre;
                    
                    _unitOfWork.PlaylistRepo.Update(_oldPlaylist);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
    

}
}
