using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace Tasks
{
    public class ApiCloudTask
    {
        public IUnitOfWork _unitOfWork;

        public ApiCloudTask(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CreateApiCloud(ApiCloud _apiCloud)
        {
            if (_apiCloud != null)
            {
                await _unitOfWork.ApiCloudRepo.Add(_apiCloud);

                var result = _unitOfWork.Save();

                return (_apiCloud.id > 0);
            }
            return false;
        }
        public async Task<bool> DeleteApiCloud(int _apiCloudId)
        {
            if (_apiCloudId > 0)
            {
                var _apiClouds = await _unitOfWork.ApiCloudRepo.GetById(_apiCloudId);
                if (_apiClouds != null)
                {
                    _unitOfWork.ApiCloudRepo.Delete(_apiClouds);
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
        public async Task<ICollection<ApiCloud>> GetAllApiClouds()
        {
            var apiCloudList = await _unitOfWork.ApiCloudRepo.GetAll();
            return apiCloudList;
        }

        public async Task<ApiCloud> GetApiCloudById(int _apiCloudId)
        {
            if (_apiCloudId > 0)
            {
                var _apiCloud = await _unitOfWork.ApiCloudRepo.GetById(_apiCloudId);
                if (_apiCloud != null)
                    return _apiCloud;

            }
            return null;
        }

        public async Task<bool> UpdateApiCloud(ApiCloud _apiCloud)
        {
            if (_apiCloud != null)
            {
                var _oldApiCloud = await _unitOfWork.ApiCloudRepo.GetById((int)_apiCloud.id);
                if (_oldApiCloud != null)
                {
                    _oldApiCloud.cle = _apiCloud.cle;
                    _oldApiCloud.fl_blocked = _apiCloud.fl_blocked;
                    _oldApiCloud.dt_block = _apiCloud.dt_block;

                    _unitOfWork.ApiCloudRepo.Update(_oldApiCloud);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
        public async Task<ICollection<ApiCloud>> getActiveApiCloud(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var apiexpired = (await _unitOfWork.ApiCloudRepo.GetAll()).Where(api => api.cle == key).FirstOrDefault();
                if(apiexpired!=null && apiexpired.id != 0)
                {
                    apiexpired.fl_blocked = true;
                    apiexpired.dt_block = DateTime.Now.ToString();
                    _=UpdateApiCloud(apiexpired);
                }
                    
            }
            var y = await _unitOfWork.ApiCloudRepo.GetAll();
            var x = (await _unitOfWork.ApiCloudRepo.GetAll()).Where(a => !a.fl_blocked ||
                         string.IsNullOrEmpty(a.dt_block) ||
                         (a.dt_block != null && (DateTime.Now - DateTime.Parse(a.dt_block)).TotalHours >= 24))
                 .Select(a => new ApiCloud()
                 {
                     id = a.id,

                     cle = a.cle,
                     type = a.type,
                     fl_blocked = a.fl_blocked,
                     dt_block = a.dt_block,

                 }).ToList();
            if (x.Where(a => a.fl_blocked == true).Count() > 0)
            {
                Thread active = new Thread(() => {
                    foreach (var y in x)
                    {
                        y.fl_blocked = false;
                        y.dt_block = "";
                        _ = UpdateApiCloud(y);
                    }

                });
                active.Start();
            }

            return x;
        }
    }
}
