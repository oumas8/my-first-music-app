using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.contract
{
    public interface IUserTask
    {
       // Task<bool> CreateUser(User _user);
        Task<User> getUserByAuth(string login,string password);
        Task<IEnumerable<User>> GetAllUsers();

        Task<User> GetUserById(int _userId);

      //Task<bool> UpdateUser(User _user);

       // Task<bool> DeleteUser(int _userId);
    }
}
