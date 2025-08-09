using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.contract
{
    public interface ISettingTask
    {
       // Task<bool> CreateSetting(Setting _Setting);
        //Task<Setting> getSettingByAuth(string login,string password);
        Task<IEnumerable<Setting>> GetAllSettings();

        Task<Setting> GetSettingById(int _SettingId);
        Task<Setting> GetSettingByUser(int _UserId);

      //Task<bool> UpdateSetting(Setting _Setting);

        // Task<bool> DeleteSetting(int _SettingId);
    }
}
