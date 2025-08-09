using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.contract
{
    public interface IkeyvalidationTask
    {
         Task<long?> Createkeyvalidation(keyvalidation _user);
        Task<keyvalidation> GetkeyvalidationById(long id);

        //Task<User> GetUserById(int _userId);

      //Task<bool> UpdateUser(User _user);

       Task<bool> Deletekeyvalidation(string key,long ?_userId);
    }
}
