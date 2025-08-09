using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasks.contract;
using System.Linq;

namespace Tasks
{
    public class MorceauTask:IMorceauTask
    {
        public IUnitOfWork _unitOfWork;

        public MorceauTask(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       
        public async Task<long> CreateMorceau(Morceau _morceau)
        {
            try
            {
                if (_morceau != null)
                {
                    await _unitOfWork.MorceauRepo.Add(_morceau);

                    var result = _unitOfWork.Save();
                    
                        return _morceau.id;
                   
                }
                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
           
            
        }
        public async Task<ICollection<Morceau>> GetPlylisteWithMorceauxAsync(int id)
        {
           return await _unitOfWork.MorceauRepo._GetPlylisteWithMorceauxAsync(id);
           
        } 
        public async Task<ICollection<Morceau>> GetPlylistMorceauAsync(string input,int idPlylist,int pageNumber,int pageSize)
        {
            return await _unitOfWork.MorceauPlaylistRepo._GetListeMorceaubyIdPlylistAsync(input,idPlylist, pageNumber, pageSize);
            
        }
        public async Task<bool> DeleteMorceau(int _morceauId)
        {
            if (_morceauId > 0)
            {
                var _morceaus = await _unitOfWork.MorceauRepo.GetById(_morceauId);
                if (_morceaus != null)
                {
                    _unitOfWork.MorceauRepo.Delete(_morceaus);
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<ICollection<Morceau>> GetAllMorceaus()
        {
            var morceauList = await _unitOfWork.MorceauRepo.GetAll();
            return morceauList;
        }

        public async Task<Morceau> GetMorceauById(int _morceauId)
        {
            if (_morceauId > 0)
            {
                var _morceau = await _unitOfWork.MorceauRepo.GetById(_morceauId);
                if (_morceau != null)
                        return _morceau;
                
            }
            return null;
        }
        public async Task<Morceau> GetMorceauByUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var _morceau = (await _unitOfWork.MorceauRepo.GetAll()).Where(m=>m.url== url).FirstOrDefault();
                if (_morceau != null && _morceau.id!=0)
                    return _morceau;

            }
            return null;
        }

        public async Task<bool> UpdateMorceau(Morceau _morceau)
        {
            if (_morceau != null)
            {
                var _oldmorceau = await _unitOfWork.MorceauRepo.GetById((int)_morceau.id);
                if (_oldmorceau != null)
                {
                    _oldmorceau.title = _morceau.title;
                    _oldmorceau.url = _morceau.url;
                    _oldmorceau.fl_local = _morceau.fl_local;
                    _oldmorceau.fl_delet = _morceau.fl_delet;
                    _oldmorceau.info = _morceau.info;
                    _oldmorceau.morceauID = _morceau.morceauID;
                   

                    _unitOfWork.MorceauRepo.Update(_oldmorceau);

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
