using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISettingRepository : IGenericRepository<Setting>
    {
        Task<Setting> GetByUser(int idUser, bool tracking = false);
    }
}
