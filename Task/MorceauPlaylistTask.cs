using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks.contract;

namespace Tasks
{
    public class MorceauPlaylistTask: IMorceauPlaylistTask
    {
        public IUnitOfWork _unitOfWork;

        public MorceauPlaylistTask(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateMorceauPlaylist(MorceauPlaylist _MorceauPlaylist)
        {
            if (_MorceauPlaylist != null && !(await isMorceauinPlaylist(_MorceauPlaylist.idMorceau, _MorceauPlaylist.idPlaylist)))
            {
                
                await _unitOfWork.MorceauPlaylistRepo.Add(_MorceauPlaylist);

                var result = _unitOfWork.Save();

                return (_MorceauPlaylist.id > 0);
                   
            }
            return false;
        }
        public async Task<bool> DeleteMorceauPlaylist(int _morceauPlaylistId)
        {
            if (_morceauPlaylistId > 0)
            {
                var _morceauPlaylists = await _unitOfWork.MorceauPlaylistRepo.GetById(_morceauPlaylistId);
                if (_morceauPlaylists != null)
                {
                    _unitOfWork.MorceauPlaylistRepo.Delete(_morceauPlaylists);
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<ICollection<MorceauPlaylist>> GetAllMorceauPlaylists()
        {
            var morceauList = await _unitOfWork.MorceauPlaylistRepo.GetAll();
            return morceauList;
        } 
        public async Task<List<MorceauPlaylist>> GetMorceauPlaylistsbyIdMorceau(int _idMorceau)
        {
            //var morceauList = await _unitOfWork.MorceauPlaylistRepo.GetAll();
            var _morceauPlylist = await _unitOfWork.MorceauPlaylistRepo.GetByIdMorceau(_idMorceau);
            return _morceauPlylist;
        }

        public async Task<MorceauPlaylist> GetMorceauPlaylistById(int _morceauPlaylistId)
        {
            if (_morceauPlaylistId > 0)
            {
                var _MorceauPlaylist = await _unitOfWork.MorceauPlaylistRepo.GetById(_morceauPlaylistId);
                if (_MorceauPlaylist != null)
                        return _MorceauPlaylist;
                
            }
            return null;
        }
        public async Task<bool> isMorceauinPlaylist(long idMorceau,long idPlylist)
        {
            var _morceauPlylist = (await _unitOfWork.MorceauPlaylistRepo.GetAll()).Where(m => m.idMorceau == idMorceau && m.idPlaylist==idPlylist).FirstOrDefault();
            return (_morceauPlylist != null && _morceauPlylist.id != 0);
        }
        public async Task<bool> UpdateMorceauPlaylist(MorceauPlaylist _MorceauPlaylist)
        {
            try
            {
            if (_MorceauPlaylist != null)
                    {
                        var _oldMorceauPlaylist = await _unitOfWork.MorceauPlaylistRepo.GetById((int)_MorceauPlaylist.id,false);
                        if (_oldMorceauPlaylist != null)
                        {
                            _oldMorceauPlaylist.place= _MorceauPlaylist.place;
                            _oldMorceauPlaylist.fl_Active = _MorceauPlaylist.fl_Active;
                            _oldMorceauPlaylist.dt_listning = _MorceauPlaylist.dt_listning;
                    
                            //_unitOfWork.MorceauPlaylistRepo.Update(_oldMorceauPlaylist);

                            var result = _unitOfWork.Save();

                            if (result > 0)
                                return true;
                            else
                                return false;
                        }
                    }
            }
            catch (Exception e)
            {
                return false;
            }
          
           return false;
        }
    

}
}
