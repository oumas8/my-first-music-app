using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks.contract;

namespace Tasks
{
    public class keyvalidationTask : IkeyvalidationTask
    {
        public IUnitOfWork _unitOfWork;
        public IkeyvalidationRepository _keyvalidationRepo;
       public keyvalidationTask(IkeyvalidationRepository IkeyvalidationRepository, IUnitOfWork unitOfWork)
      {
            _keyvalidationRepo = IkeyvalidationRepository;
          _unitOfWork = unitOfWork;
    }
        public async Task<long?> Createkeyvalidation(keyvalidation _keyvalidation)
        {
            try
            {
   if (_keyvalidation != null)
            {
                await _unitOfWork.keyvalidationRepo.Add(_keyvalidation);

                //var result = _unitOfWork.Save();
                    return _keyvalidation.id;
                
            }
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
         
            return null;
        }
        public async Task<bool> Deletekeyvalidation(string _keyvalidationId,long? idUser)
        {
            
                
                var _keyvalidations = (await _unitOfWork.keyvalidationRepo.GetAll()).Where(x=>
                    (!string.IsNullOrEmpty(_keyvalidationId))?x.key!= _keyvalidationId:true
                    && (idUser!=null) ?x.idUser== idUser : true
                    ).ToList();


                if (_keyvalidations != null && _keyvalidations.Count>0)
                {
                    foreach(var key in _keyvalidations)
                    {
                        _unitOfWork.keyvalidationRepo.DeleteKeyById(key.id);
                    }
                   return true;
                }
            
            return false;
        }

      /*  public async Task<IEnumerable<keyvalidation>> GetAllkeyvalidations()
        {
            var keyvalidationList = await _keyvalidationRepo.GetAll();
            return null;
        }*/

       public async Task<keyvalidation> GetkeyvalidationById(long _keyvalidationId)
        {
            if (_keyvalidationId > 0)
            {
                var _keyvalidation = await _unitOfWork.keyvalidationRepo.GetById((int)_keyvalidationId);
                if (_keyvalidation != null)
                        return _keyvalidation;
                
            }
            return null;
        }

       /* public async Task<bool> Updatekeyvalidation(keyvalidation _keyvalidation)
        {
            if (_keyvalidation != null)
            {
                var _oldkeyvalidation = await _unitOfWork.keyvalidationRepo.GetById((int)_keyvalidation.id);
                if (_oldkeyvalidation != null)
                {
                    _oldkeyvalidation.name = _keyvalidation.name;
                    _oldkeyvalidation.login = _keyvalidation.login;
                    _oldkeyvalidation.password = _keyvalidation.password;
                    
                    _unitOfWork.keyvalidationRepo.Update(_oldkeyvalidation);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }*/
 
}
}
