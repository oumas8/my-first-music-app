using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks.contract;

namespace Tasks
{
    public class SettingTask : ISettingTask
    {
        public IUnitOfWork _unitOfWork;
        public ISettingRepository _SettingRepo;
       public SettingTask(ISettingRepository ISettingRepository, IUnitOfWork unitOfWork)
      {
            _SettingRepo = ISettingRepository;
          _unitOfWork = unitOfWork;
    }

        public async Task<bool> CreateSetting(Setting _Setting)
        {
            if (_Setting != null)
            {
                await _unitOfWork.SettingRepo.Add(_Setting);

                var result = _unitOfWork.Save();

                    return _Setting.id>0;
                
            }
            return false;
        }
        public async Task<bool> DeleteSetting(int _SettingId)
        {
            if (_SettingId > 0)
            {
                var _Settings = await _unitOfWork.SettingRepo.GetById(_SettingId);
                if (_Settings != null)
                {
                    _unitOfWork.SettingRepo.Delete(_Settings);
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<IEnumerable<Setting>> GetAllSettings()
        {
            var SettingList = await _SettingRepo.GetAll();
            return null;
        }

       public async Task<Setting> GetSettingById(int _SettingId)
        {
            if (_SettingId > 0)
            {
                var _Setting = await _unitOfWork.SettingRepo.GetById(_SettingId);
                if (_Setting != null)
                        return _Setting;
                
            }
            return null;
        } 
        public async Task<Setting> GetSettingByUser(int _UserId)
        {
            if (_UserId > 0)
            {
                var _Setting = await _unitOfWork.SettingRepo.GetByUser(_UserId);
                if (_Setting != null)
                        return _Setting;
                
            }
            return null;
        }

        public async Task<bool> UpdateSetting(Setting _Setting)
        {
            if (_Setting != null)
            {
                    _unitOfWork.SettingRepo.Update(_Setting);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                
            }
            return false;
        }
    //public async Task<Setting> getSettingByAuth(string login,string password)
    //    {
    //        var Setting = (await _unitOfWork.SettingRepo.GetAll()).Where(x=>x.login==login && x.password==password).FirstOrDefault();
    //        return Setting;
    //    }

}
}
