using Domain.Interfaces;
using Domain.Models.Enteties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repositories
{
     public class SettingRepository : GenericRepository<Setting>, ISettingRepository
    {
        public SettingRepository(DbContextClass dbContext) : base(dbContext)
        {

        }
        public async Task<Setting> GetByUser(int idUser, bool tracking = false)
        {
            return await _dbContext.Setting.Where(m => m.idUser == idUser).FirstOrDefaultAsync();
        }
    }
}
