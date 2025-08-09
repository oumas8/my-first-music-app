using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks.contract;

namespace Tasks
{
    public class UserTask: IUserTask
    {
        public IUnitOfWork _unitOfWork;
        public IUserRepository _userRepo;
       public UserTask(IUserRepository IUserRepository, IUnitOfWork unitOfWork)
      {
            _userRepo = IUserRepository;
          _unitOfWork = unitOfWork;
    }

        public async Task<bool> CreateUser(User _user)
        {
            if (_user != null)
            {
                await _unitOfWork.UserRepo.Add(_user);

                var result = _unitOfWork.Save();

                    return _user.id>0;
                
            }
            return false;
        }
        public async Task<bool> DeleteUser(int _userId)
        {
            if (_userId > 0)
            {
                var _users = await _unitOfWork.UserRepo.GetById(_userId);
                if (_users != null)
                {
                    _unitOfWork.UserRepo.Delete(_users);
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var userList = await _userRepo.GetAll();
            return null;
        }

       public async Task<User> GetUserById(int _userId)
        {
            if (_userId > 0)
            {
                var _user = await _unitOfWork.UserRepo.GetById(_userId);
                if (_user != null)
                        return _user;
                
            }
            return null;
        }

        public async Task<bool> UpdateUser(User _user)
        {
            if (_user != null)
            {
                var _oldUser = await _unitOfWork.UserRepo.GetById((int)_user.id);
                if (_oldUser != null)
                {
                    _oldUser.name = _user.name;
                    _oldUser.login = _user.login;
                    _oldUser.password = _user.password;
                    
                    _unitOfWork.UserRepo.Update(_oldUser);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
    public async Task<User> getUserByAuth(string login,string password)
        {
            var user = (await _unitOfWork.UserRepo.GetAll()).Where(x=>x.login==login && x.password==password).FirstOrDefault();
            return user;
        }

}
}
